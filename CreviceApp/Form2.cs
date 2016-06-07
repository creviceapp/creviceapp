using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp
{
    public partial class Form2 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        static extern int GetObject(IntPtr hgdiobj, int cbBuffer, ref BITMAP lpvObject);

        [DllImport("gdi32.dll")]
        static extern IntPtr GetCurrentObject(IntPtr hdc, uint uObjectType);
        
        [DllImport("gdi32.dll")]
        public static extern int DeleteObject(IntPtr hobject);

        [DllImport("gdi32.dll")]
        public static extern int DeleteDC(IntPtr hdc);
        
        public const int OBJ_PEN    = 1;
        public const int OBJ_BITMAP = 7;
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int UpdateLayeredWindow(
            IntPtr hwnd, 
            IntPtr hdcDst,
            [In()] ref Point pptDst,
            [In()] ref Size psize,
            IntPtr hdcSrc,
            [In()] ref Point pptSrc,
            int crKey,
            [In()] ref BLENDFUNCTION pblend,
            int dwFlags);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        public const byte AC_SRC_OVER  = 0;
        public const byte AC_SRC_ALPHA = 1;
        public const int  ULW_ALPHA    = 2;
        
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        
        public enum DeviceCap
        {
            /// <summary>
            /// Device driver version
            /// </summary>
            DRIVERVERSION = 0,
            /// <summary>
            /// Device classification
            /// </summary>
            TECHNOLOGY = 2,
            /// <summary>
            /// Horizontal size in millimeters
            /// </summary>
            HORZSIZE = 4,
            /// <summary>
            /// Vertical size in millimeters
            /// </summary>
            VERTSIZE = 6,
            /// <summary>
            /// Horizontal width in pixels
            /// </summary>
            HORZRES = 8,
            /// <summary>
            /// Vertical height in pixels
            /// </summary>
            VERTRES = 10,
            /// <summary>
            /// Number of bits per pixel
            /// </summary>
            BITSPIXEL = 12,
            /// <summary>
            /// Number of planes
            /// </summary>
            PLANES = 14,
            /// <summary>
            /// Number of brushes the device has
            /// </summary>
            NUMBRUSHES = 16,
            /// <summary>
            /// Number of pens the device has
            /// </summary>
            NUMPENS = 18,
            /// <summary>
            /// Number of markers the device has
            /// </summary>
            NUMMARKERS = 20,
            /// <summary>
            /// Number of fonts the device has
            /// </summary>
            NUMFONTS = 22,
            /// <summary>
            /// Number of colors the device supports
            /// </summary>
            NUMCOLORS = 24,
            /// <summary>
            /// Size required for device descriptor
            /// </summary>
            PDEVICESIZE = 26,
            /// <summary>
            /// Curve capabilities
            /// </summary>
            CURVECAPS = 28,
            /// <summary>
            /// Line capabilities
            /// </summary>
            LINECAPS = 30,
            /// <summary>
            /// Polygonal capabilities
            /// </summary>
            POLYGONALCAPS = 32,
            /// <summary>
            /// Text capabilities
            /// </summary>
            TEXTCAPS = 34,
            /// <summary>
            /// Clipping capabilities
            /// </summary>
            CLIPCAPS = 36,
            /// <summary>
            /// Bitblt capabilities
            /// </summary>
            RASTERCAPS = 38,
            /// <summary>
            /// Length of the X leg
            /// </summary>
            ASPECTX = 40,
            /// <summary>
            /// Length of the Y leg
            /// </summary>
            ASPECTY = 42,
            /// <summary>
            /// Length of the hypotenuse
            /// </summary>
            ASPECTXY = 44,
            /// <summary>
            /// Shading and Blending caps
            /// </summary>
            SHADEBLENDCAPS = 45,

            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LOGPIXELSX = 88,
            /// <summary>
            /// Logical pixels inch in Y
            /// </summary>
            LOGPIXELSY = 90,

            /// <summary>
            /// Number of entries in physical palette
            /// </summary>
            SIZEPALETTE = 104,
            /// <summary>
            /// Number of reserved entries in palette
            /// </summary>
            NUMRESERVED = 106,
            /// <summary>
            /// Actual color resolution
            /// </summary>
            COLORRES = 108,

            // Printing related DeviceCaps. These replace the appropriate Escapes
            /// <summary>
            /// Physical Width in device units
            /// </summary>
            PHYSICALWIDTH = 110,
            /// <summary>
            /// Physical Height in device units
            /// </summary>
            PHYSICALHEIGHT = 111,
            /// <summary>
            /// Physical Printable Area x margin
            /// </summary>
            PHYSICALOFFSETX = 112,
            /// <summary>
            /// Physical Printable Area y margin
            /// </summary>
            PHYSICALOFFSETY = 113,
            /// <summary>
            /// Scaling factor x
            /// </summary>
            SCALINGFACTORX = 114,
            /// <summary>
            /// Scaling factor y
            /// </summary>
            SCALINGFACTORY = 115,

            /// <summary>
            /// Current vertical refresh rate of the display device (for displays only) in Hz
            /// </summary>
            VREFRESH = 116,
            /// <summary>
            /// Vertical height of entire desktop in pixels
            /// </summary>
            DESKTOPVERTRES = 117,
            /// <summary>
            /// Horizontal width of entire desktop in pixels
            /// </summary>
            DESKTOPHORZRES = 118,
            /// <summary>
            /// Preferred blt alignment
            /// </summary>
            BLTALIGNMENT = 119

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public ushort bmPlanes;
            public ushort bmBitsPixel;
            public IntPtr bmBits;
        };

        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_TOOLWINDOW  = 0x00000080;
        public const int WS_EX_LAYERED     = 0x00080000;
        public const int WS_EX_NOACTIVATE  = 0x08000000;

        private LocalDeviceContext ldc;
        
        public Form2()
        {
            InitializeComponent();
            this.Name = "CreviceAppGestureStrokeOverlay";
            this.Text = "";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            this.ldc = new LocalDeviceContext();
            Debug.Print("LocalDeviceContext: {0}", this.ldc.GetBounds());
        }

        public void UpdateLocalDeviceContext()
        {
            var hdc = GetDC(IntPtr.Zero);
            if (!ldc.IsCompatible(hdc))
            {
                this.ldc.Dispose();
                this.ldc = new LocalDeviceContext(hdc);
            }
        }
        
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle = WS_EX_TOOLWINDOW | WS_EX_LAYERED | WS_EX_NOACTIVATE | WS_EX_TRANSPARENT;
                return cp;
            }
        }

        public void Update(IEnumerable<LowLevelMouseHook.POINT> points)
        {

        }

        class LocalDeviceContext : IDisposable
        {
            public readonly IntPtr Handle;

            public LocalDeviceContext() 
            {
                var hdc = GetDC(IntPtr.Zero);
                try
                {
                    this.Handle = CreateCompatibleDC(hdc);
                }
                finally
                {
                    ReleaseDC(IntPtr.Zero, hdc);
                }
            }
            
            public LocalDeviceContext(IntPtr hdc)
            {
                this.Handle = CreateCompatibleDC(hdc);
            }

            public Rectangle GetBounds()
            {
                return GetBounds(Handle);
            }

            public Rectangle GetBounds(IntPtr hdc)
            {
                var obj = GetCurrentObject(hdc, OBJ_BITMAP);
                var bmp = new BITMAP();
                GetObject(obj, Marshal.SizeOf(bmp), ref bmp);
                return new Rectangle(0, 0, bmp.bmWidth, bmp.bmHeight);
            }

            public bool IsCompatible(IntPtr hdc)
            {
                var a = GetBounds(Handle);
                var b = GetBounds(hdc);
                Debug.Print("a: {0}", a);
                Debug.Print("b: {0}", b);
                return a.Width == b.Width &&
                       a.Height == b.Height;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                DeleteDC(Handle);
            }

            ~LocalDeviceContext()
            {
                Dispose();
            }
        }
    }
}
