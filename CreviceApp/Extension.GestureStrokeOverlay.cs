using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.Extension.GestureStrokeOverlay
{
    using System.Drawing;
    using System.Threading;
    using System.Collections.Concurrent;
    using Crevice.Logging;
    using Crevice.WinAPI.Window;

    class Message { }
    class ResetMessage : Message { }
    class DrawMessage : Message
    {
        public readonly IReadOnlyList<Core.Stroke.Stroke> Strokes;
        public readonly IReadOnlyList<Point> BufferedPoints;
        public DrawMessage(IReadOnlyList<Core.Stroke.Stroke> strokes, IReadOnlyList<Point> bufferedPoints)
        {
            this.Strokes = strokes;
            this.BufferedPoints = bufferedPoints;
        }
    }

    class OverlayForm : Form
    {
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | WS_EX_LAYERED | WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        public void InvokeProperly(MethodInvoker invoker)
        {
            if (InvokeRequired)
            {
                Invoke(invoker);
            }
            else
            {
                invoker.Invoke();
            }
        }

        private readonly Pen _normalLinePen;
        private readonly Pen _newLinePen;
        private readonly Pen _undeterminedLinePen;
        private readonly float _lineWidth;

        private readonly GestureStrokeOverlay _manager;

        private readonly object _lockObject = new object();

        public OverlayForm(
            GestureStrokeOverlay manager,
            Color normalLineColor,
            Color newLineColor,
            Color undeterminedLineColor,
            float lineWidth)
        {
            this._manager = manager;
            this._normalLinePen = GetPrefferedPen(normalLineColor, lineWidth);
            this._newLinePen = GetPrefferedPen(newLineColor, lineWidth);
            this._undeterminedLinePen = GetPrefferedPen(undeterminedLineColor, lineWidth);
            this._lineWidth = lineWidth;
            this.MaximumSize = new Size(int.MaxValue, int.MaxValue);
            UpdateSizeAndLocation();
            InitializeComponent();
        }

        private Pen GetPrefferedPen(Color color, float width)
            => new Pen(color, width)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.Round,
                LineJoin = System.Drawing.Drawing2D.LineJoin.Round
            };

        private int _maxRenderedStrokeId = 0;

        private HashSet<Point> _renderedPoints = new HashSet<Point>();
        private Dictionary<int, Rectangle> _strokeRectCache = new Dictionary<int, Rectangle>();
        private Rectangle _undeterminedRect = new Rectangle();

        private CancellationTokenSource _tokenSource = null;

        private void RequestCancel()
        {
            if (_tokenSource == null) return;
            try
            {
                _tokenSource.Cancel();
            }
            catch { }
        }

        public void Reset() =>
            InvokeProperly(() =>
            {
                RequestCancel();
                lock (_lockObject)
                {
                    Clear(GetRectFromPoints(_renderedPoints));
                    Clear(_undeterminedRect);
                    this.TopMost = true;
                    this.TopLevel = true;
                    _maxRenderedStrokeId = 0;
                    _renderedPoints = new HashSet<Point>();
                    _strokeRectCache = new Dictionary<int, Rectangle>();
                    _undeterminedRect = new Rectangle();
                }
            });

        public void Draw(DrawMessage dm) =>
            InvokeProperly(() =>
            {
                RequestCancel();
                lock (_lockObject)
                {
                    if (_maxRenderedStrokeId == 0) UpdateSizeAndLocation();
                    DrawStroke(dm);
                }
            });

        private void UpdateSizeAndLocation()
        {
            var workingArea = Screen.GetWorkingArea(Window.GetPhysicalCursorPos());
            this.Size = workingArea.Size;
            this.Location = workingArea.Location;
        }

        private Rectangle GetRectFromPoints(IEnumerable<Point> points)
        {
            if (points.Count() < 2) return new Rectangle(0, 0, 0, 0);
            var margin = _lineWidth * 10;
            var x = points.Select(p => p.X).Min() - margin;
            var y = points.Select(p => p.Y).Min() - margin;
            var w = points.Select(p => p.X).Max() - x + margin * 2;
            var h = points.Select(p => p.Y).Max() - y + margin * 2;
            return new Rectangle((int)x, (int)y, (int)w, (int)h);
        }

        private Rectangle GetRectFromLines(IEnumerable<IEnumerable<Point>> lines)
            => lines.Select(x => GetRectFromPoints(x)).Aggregate((x, y) => Rectangle.Union(x, y));

        private void Clear(Rectangle rect)
        {
            using (var g = CreateGraphics())
            using (var buffer = BufferedGraphicsManager.Current.Allocate(g, rect))
            {
                buffer.Graphics.Clear(Color.Transparent);
                buffer.Render();
            }
        }

        private void DrawStroke(DrawMessage dm)
        {
            _tokenSource = new CancellationTokenSource();
            try
            {
                using (var g = CreateGraphics())
                using (var b = BufferedGraphicsManager.Current.Allocate(g, ToRelativeRect(_undeterminedRect)))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                    b.Graphics.Clear(Color.Transparent);

                    var gs0 = new Graphics[] { b.Graphics };
                    var gs1 = new Graphics[] { g, b.Graphics };

                    var strokes = dm.Strokes;
                    foreach (var (v, i) in strokes.Select((v, i) => (v, i)))
                    {
                        if (_tokenSource.IsCancellationRequested) return;

                        var isNormal = i < _maxRenderedStrokeId;
                        var hasRectCache = _strokeRectCache.ContainsKey(i);

                        if (!v.Points.Any() ||
                            isNormal && hasRectCache && 
                            !_undeterminedRect.IntersectsWith(_strokeRectCache[i])) continue;

                        var lines = GetDeterminedLines(strokes, i).ToList();
                        if (isNormal)
                        {
                            var gs = isNormal && hasRectCache ? gs0 : gs1;
                            DrawNormalLines(lines, gs);
                            _strokeRectCache[i] = GetRectFromLines(lines);
                        }
                        else
                        {
                            DrawNewLines(lines, gs1);
                        }
                        _maxRenderedStrokeId = Math.Max(_maxRenderedStrokeId, i);
                    }

                    if (_tokenSource.IsCancellationRequested) return;

                    var bufferedPoints = dm.BufferedPoints;
                    if (bufferedPoints.Any())
                    {
                        var lines = GetUndeterminedLines(strokes, bufferedPoints).ToList();
                        DrawUndeterminedLines(lines, gs1);
                        _undeterminedRect = GetRectFromLines(lines);
                    }

                    b.Render();
                }
            }
            finally
            {
                _tokenSource.Dispose();
                _tokenSource = null;
            }
        }

        private IEnumerable<IEnumerable<Point>> GetDeterminedLines(IReadOnlyList<Core.Stroke.Stroke> strokes, int pos)
        {
            var xs = strokes[pos].Points;
            if (xs.Any())
            {
                if (pos > 0)
                {
                    var prev = strokes[pos - 1].Points;
                    if (prev.Any())
                    {
                        yield return new Point[] { prev.Last(), xs.First() };
                    }
                }
                if (xs.Count > 1)
                {
                    yield return xs;
                }
            }
        }

        private IEnumerable<IEnumerable<Point>> GetUndeterminedLines(IReadOnlyList<Core.Stroke.Stroke> strokes, IReadOnlyList<Point> buf)
        {
            if (buf.Any())
            {
                if (strokes.Any())
                {
                    var last = strokes.Last().Points;
                    if (last.Any())
                    {
                        yield return new Point[] { last.Last(), buf.First() };
                    }
                }
                if (buf.Count > 1)
                {
                    yield return buf;
                }
                yield return new Point[] { buf.Last(), Cursor.Position };
            }
        }

        private Point[] ToRelativePoint(IEnumerable<Point> points) 
            => points.Select(p => new Point(p.X - Location.X, p.Y - Location.Y)).ToArray();

        private Rectangle ToRelativeRect(Rectangle rect)
            => new Rectangle(rect.X - Location.X, rect.Y - Location.Y, rect.Width, rect.Height);

        private void DrawLines(Pen pen, IEnumerable<IEnumerable<Point>> lines, params Graphics[] gs)
        {
            foreach (var points in lines)
            {
                var relativePoints = ToRelativePoint(points);
                foreach (var g in gs)
                {
                    g.DrawLines(pen, relativePoints);
                }
                foreach (var p in relativePoints)
                {
                    _renderedPoints.Add(p);
                }
            }
        }

        private void DrawNormalLines(IEnumerable<IEnumerable<Point>> lines, params Graphics[] gs)
            =>  DrawLines(_normalLinePen, lines, gs);

        private void DrawNewLines(IEnumerable<IEnumerable<Point>> lines, params Graphics[] gs) 
            => DrawLines(_newLinePen, lines, gs);

        private void DrawUndeterminedLines(IEnumerable<IEnumerable<Point>> lines, params Graphics[] gs)
            => DrawLines(_undeterminedLinePen, lines, gs);

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _normalLinePen.Dispose();
                _newLinePen.Dispose();
                _undeterminedLinePen.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Name = "GestureStrokeOverlayForm";
            this.Text = this.Name;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.TopLevel = true;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.DoubleBuffered = true;
            this.ResumeLayout(false);
        }
    }

    public class GestureStrokeOverlay : IDisposable
    {

        private readonly object _lockObject = new object();

        private Core.Stroke.StrokeWatcher _currentStrokeWatcher { get; set; } = null;

        public void Reset(Core.Stroke.StrokeWatcher strokeWatcher)
        {
            try
            {
                lock (_lockObject)
                {
                    if (_disposed) return;
                    _currentStrokeWatcher = strokeWatcher;
                    var message = new ResetMessage();
                    _messageQueue.Add(message);
                }
            }
            catch (InvalidOperationException) { }
        }

        public void Draw(Core.Stroke.StrokeWatcher strokeWatcher, IReadOnlyList<Core.Stroke.Stroke> strokes, IReadOnlyList<Point> bufferedPoints)
        {
            try
            {
                lock (_lockObject)
                {
                    if (_disposed) return;
                    if (_currentStrokeWatcher != strokeWatcher) return;
                    var message = new DrawMessage(strokes, bufferedPoints);
                    _messageQueue.Add(message);
                }
            }
            catch (InvalidOperationException) { }
        }

        private readonly BlockingCollection<Message> _messageQueue = new BlockingCollection<Message>();

        private readonly OverlayForm _form;

        public GestureStrokeOverlay(Color normalLineColor, Color newLineColor, Color undeterminedLineColor, float lineWidth)
        {
            _form = new OverlayForm(this, normalLineColor, newLineColor, undeterminedLineColor, lineWidth);
            _form.Show();

            RunMessageProcessTask();
        }

        private void RunMessageProcessTask() =>
            Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (var message in _messageQueue.GetConsumingEnumerable())
                    {
                        if (_disposed) break;
                        try
                        {
                            if (message is ResetMessage)
                            {
                                _form.Reset();
                            }
                            else if (message is DrawMessage dm)
                            {
                                _form.Draw(dm);
                            }
                        }
                        catch (Exception ex)
                        {
                            Verbose.Error($"Error on MessageProcessTask: {ex.ToString()}");
                        }
                    }
                }
                catch (OperationCanceledException) { }
                finally
                {
                    _form.InvokeProperly(() =>
                    {
                        _form.Close();
                        _form.Dispose();
                    });
                    try
                    {
                        _messageQueue.Dispose();
                    }
                    catch { }
                }
            });

        internal bool _disposed { get; private set; } = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (_lockObject)
            {
                if (_disposed) return;
                _disposed = true;
                if (disposing)
                {
                    try
                    {
                        _messageQueue.CompleteAdding();
                    }
                    catch { }
                }
            }
        }

        ~GestureStrokeOverlay() => Dispose(false);
    }
}
