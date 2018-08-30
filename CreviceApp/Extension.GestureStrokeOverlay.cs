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
    using System.Collections.Concurrent;
    using Crevice.Logging;

    class Message { }
    class ResetMessage : Message { }
    class DrawMessage : Message
    {
        public readonly IEnumerable<Core.Stroke.Stroke> Strokes;
        public readonly IEnumerable<Point> BufferedPoints;
        public DrawMessage(IEnumerable<Core.Stroke.Stroke> strokes, IEnumerable<Point> bufferedPoints)
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
            Size size, 
            Point location,
            Color normalLineColor,
            Color newLineColor,
            Color undeterminedLineColor,
            float lineWidth)
        {
            this._manager = manager;
            this._normalLinePen = new Pen(normalLineColor, lineWidth);
            this._newLinePen = new Pen(newLineColor, lineWidth);
            this._undeterminedLinePen = new Pen(undeterminedLineColor, lineWidth);
            this._lineWidth = lineWidth;
            this.MaximumSize = size;
            this.ClientSize = size;
            this.Size = size;
            this.Location = location;
            InitializeComponent();
        }

        private int _maxRenderedStrokeId = 0;

        private HashSet<Point> renderedPoints = new HashSet<Point>();

        public void Reset() =>
            InvokeProperly(() =>
            {
                lock (_lockObject)
                {
                    ClearTaintedArea();
                    this.TopMost = true;
                    this.TopLevel = true;
                    _maxRenderedStrokeId = 0;
                    renderedPoints = new HashSet<Point>();
                }
            });

        public void Draw(DrawMessage dm) =>
            InvokeProperly(() =>
            {
                lock (_lockObject)
                {
                    DrawStroke(dm);
                }
            });

        private void ClearTaintedArea()
        {
            if (renderedPoints.Count <= 2) return;
            var margin = Math.Max(100, _lineWidth * 10);
            var x = renderedPoints.Select(p => p.X).Min() - margin;
            var y = renderedPoints.Select(p => p.Y).Min() - margin;
            var w = renderedPoints.Select(p => p.X).Max() - x + margin * 2;
            var h = renderedPoints.Select(p => p.Y).Max() - y + margin * 2;
            var rect = new Rectangle((int)x, (int)y, (int)w, (int)h);
            using (var g = CreateGraphics())
            using (var buffer = BufferedGraphicsManager.Current.Allocate(g, rect))
            {
                buffer.Graphics.Clear(Color.Transparent);
                buffer.Render();
            }
        }

        private void DrawStroke(DrawMessage dm)
        {
            using (var g = CreateGraphics())
            {
                var strokes = dm.Strokes;
                var bufferedPoints = dm.BufferedPoints;
                foreach (var (v, i) in dm.Strokes.Select((v, i) => (v, i)))
                {
                    // Previous last stroke should be re-rendered for the case it is undetermined stroke.
                    if (i < _maxRenderedStrokeId)
                    {
                        Verbose.Print($"Stroke({i + 1}/{strokes.Count()}) was skipped.");
                        continue;
                    }
                    var pen = v != strokes.Last() ? _normalLinePen : _newLinePen;
                    var points = v.Points.Select(p => new Point(p.X - Location.X, p.Y - Location.Y)).ToArray();
                    Verbose.Print($"Drawing Stroke({i + 1}/{strokes.Count()}) of {points.Count()} points.");
                    DrawLines(g, pen, points);
                }
                if (bufferedPoints.Any())
                {
                    var points = new List<Point>();
                    if (strokes.Any() && strokes.Last().Points.Any())
                    {
                        points.Add(strokes.Last().Points.Last());
                    }
                    points.AddRange(bufferedPoints);
                    points.Add(Cursor.Position);
                    
                    Verbose.Print($"Drawing undetermined stroke of {points.Count()} points.");
                    DrawLines(g, _undeterminedLinePen, points.Select(p => new Point(p.X - Location.X, p.Y - Location.Y)).ToArray());
                }
                _maxRenderedStrokeId = strokes.Count() - 1;
            }
        }

        private void DrawLines(Graphics g, Pen pen, Point[] points)
        {
            g.DrawLines(pen, points);
            foreach (var p in points)
            {
                renderedPoints.Add(p);
            }
        }

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

        public void Draw(Core.Stroke.StrokeWatcher strokeWatcher, IEnumerable<Core.Stroke.Stroke> strokes, IEnumerable<Point> bufferedPoints)
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
            var screens = Screen.AllScreens;
            var overlayLocation = new Point(
                screens.Select(s => s.Bounds.X).Min(), 
                screens.Select(s => s.Bounds.Y).Min());
            var overlaySize = new Size(
                screens.Select(s => s.Bounds.X + s.Bounds.Width).Max() - overlayLocation.X, 
                screens.Select(s => s.Bounds.Y + s.Bounds.Height).Max() - overlayLocation.Y);

            _form = new OverlayForm(this, overlaySize, overlayLocation, normalLineColor, newLineColor, undeterminedLineColor, lineWidth);
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
                            Verbose.Print($"Error on MessageProcessTask: {ex.ToString()}");
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
