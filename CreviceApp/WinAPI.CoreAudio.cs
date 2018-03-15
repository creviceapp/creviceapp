using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Crevice.WinAPI.CoreAudio
{
    public class VolumeControl : IDisposable
    {
        // http://www.pinvoke.net/default.aspx/Enums/CLSCTX.html
        [Flags]
        internal enum CLSCTX
        {
            CLSCTX_INPROC_SERVER          = 0x1,
            CLSCTX_INPROC_HANDLER         = 0x2,
            CLSCTX_LOCAL_SERVER           = 0x4,
            CLSCTX_INPROC_SERVER16        = 0x8,
            CLSCTX_REMOTE_SERVER          = 0x10,
            CLSCTX_INPROC_HANDLER16       = 0x20,
            CLSCTX_RESERVED1              = 0x40,
            CLSCTX_RESERVED2              = 0x80,
            CLSCTX_RESERVED3              = 0x100,
            CLSCTX_RESERVED4              = 0x200,
            CLSCTX_NO_CODE_DOWNLOAD       = 0x400,
            CLSCTX_RESERVED5              = 0x800,
            CLSCTX_NO_CUSTOM_MARSHAL      = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD   = 0x2000,
            CLSCTX_NO_FAILURE_LOG         = 0x4000,
            CLSCTX_DISABLE_AAA            = 0x8000,
            CLSCTX_ENABLE_AAA             = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT   = 0x20000,
            CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000,
            CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER    | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL    = CLSCTX_SERVER        | CLSCTX_INPROC_HANDLER
        }

        // https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd370828(v=vs.85).aspx
        internal enum EDataFlow
        {
            eRender,
            eCapture,
            eAll,
            EDataFlow_enum_count
        }

        // https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd370842(v=vs.85).aspx

        internal enum ERole
        {
            eConsole,
            eMultimedia,
            eCommunications,
            ERole_enum_count
        }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IPropertyStore
        {

        }

        [Guid("657804FA-D6AD-4496-8A60-352752AF4F89"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioEndpointVolumeCallback
        {
            [PreserveSig]
            int OnNotify(IntPtr pNotifyData);
        }

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IMMDeviceEnumerator
        {
            [PreserveSig]
            int EnumAudioEndpoints(EDataFlow dataFlow, uint StateMark, out IMMDeviceCollection device);

            [PreserveSig]
            int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);

            [PreserveSig]
            int GetDevice(string pwstrId, out IMMDevice ppDevice);

            [PreserveSig]
            int RegisterEndpointNotificationCallback(IntPtr pClient);

            [PreserveSig]
            int UnregisterEndpointNotificationCallback(IntPtr pClient);
        }
        internal static Guid MMDeviceEnumerator = new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E");

        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IMMDevice
        {
            [PreserveSig]
            int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

            [PreserveSig]
            int OpenPropertyStore(int stgmAccess, out IPropertyStore propertyStore);

            [PreserveSig]
            int GetId(int stgmAccess, [MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);
            
            [PreserveSig]
            int GetState(out int pdwState);
        }
        internal static Guid IID_IAudioEndpointVolume = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");
        
        [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IMMDeviceCollection
        {
            [PreserveSig]
            int GetCount(out uint pcDevices);

            [PreserveSig]
            int Item(uint nDevice, out IMMDevice Device);
        }

        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioEndpointVolume
        {

            [PreserveSig]
            int RegisterControlChangeNotify(IAudioEndpointVolumeCallback pNotify);

            [PreserveSig]
            int UnregisterControlChangeNotify(IAudioEndpointVolumeCallback pNotify);

            [PreserveSig]
            int GetChannelCount(ref int pnChannelCount);

            [PreserveSig]
            int SetMasterVolumeLevel(float fLevelDB, Guid pguidEventContext);

            [PreserveSig]
            int SetMasterVolumeLevelScalar(float fLevelDB, Guid pguidEventContext);

            [PreserveSig]
            int GetMasterVolumeLevel(out float fLevelDB);

            [PreserveSig]
            int GetMasterVolumeLevelScalar(out float fLevelDB);

            [PreserveSig]
            int SetChannelVolumeLevel(uint nChannel, out float fLevelDB, Guid pguidEventContext);

            [PreserveSig]
            int SetChannelVolumeLevelScalar(uint nChannel, out float fLevelDB, Guid pguidEventContext);

            [PreserveSig]
            int GetChannelVolumeLevel(uint nChannel, out float fLevelDB);

            [PreserveSig]
            int GetChannelVolumeLevelScalar(uint nChannel, out float fLevelDB);

            [PreserveSig]
            int SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, Guid pguidEventContext);

            [PreserveSig]
            int GetMute([MarshalAs(UnmanagedType.Bool)] out bool bMute);

            [PreserveSig]
            int GetVolumeStepInfo(uint pnStep, out uint pnStepCount);

            [PreserveSig]
            int VolumeStepUp(Guid pguidEventContext);
            
            [PreserveSig]
            int VolumeStepDown(Guid pguidEventContext);

            [PreserveSig]
            int QueryHardwareSupport(ref uint pdwHardwareSupportMask);
            
            [PreserveSig]
            int GetVolumeRange(out float pflVolumeMindB, out float pflVolumeMaxdB, out float pflVolumeIncrementdB);
        }

        private readonly IMMDeviceEnumerator enumerator;

        public VolumeControl()
        {
            enumerator = (IMMDeviceEnumerator)Activator.CreateInstance(Type.GetTypeFromCLSID(MMDeviceEnumerator));
        }

        internal IAudioEndpointVolume GetAudioEndpointVolume()
        {
            IMMDevice speakers;
            Marshal.ThrowExceptionForHR(enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers));
            try
            {
                object endpoint;
                Marshal.ThrowExceptionForHR(speakers.Activate(ref IID_IAudioEndpointVolume, (int)CLSCTX.CLSCTX_ALL, IntPtr.Zero, out endpoint));    
                return endpoint as IAudioEndpointVolume;
            }
            finally
            {
                Marshal.ReleaseComObject(speakers);
            }
        }
        
        public float GetMasterVolume()
        {
            var endpoint = GetAudioEndpointVolume();
            try
            {
                float level;
                Marshal.ThrowExceptionForHR(endpoint.GetMasterVolumeLevelScalar(out level));
                return level;
            }
            finally
            {
                Marshal.ReleaseComObject(endpoint);
            }
        }

        public void SetMasterVolume(float value)
        {
            var endpoint = GetAudioEndpointVolume();
            try
            {
                Marshal.ThrowExceptionForHR(endpoint.SetMasterVolumeLevelScalar(NormalizeVolume(value), Guid.Empty));
            }
            finally
            {
                Marshal.ReleaseComObject(endpoint);
            }
        }

        private float NormalizeVolume(float value)
        {
            if (value > 1)
            {
                return 1;
            }
            else if (value < 0)
            {
                return 0;
            }
            return value;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Marshal.ReleaseComObject(enumerator);
            }
        }

        ~VolumeControl() => Dispose(false);
    }
}