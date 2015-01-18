using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogiSDK
{
    #region Events
    /// <summary>
    /// Event for when ARX sends a log message through the event 'ErrorMessage'
    /// </summary>
    public class LogEvent : EventArgs
    {
        public string Message { get; private set; }
        public LogEvent(string message)
        {
            Message = message;
        }
    }
    /// <summary>
    /// Event for when the orientation on the device changes 
    /// </summary>
    public class OrientationEvent : EventArgs
    {
        public Orientation Orient { get; private set; }
        public OrientationEvent(Orientation orient)
        {
            Orient = orient;
        }
    }
    /// <summary>
    /// Event for when an html element was clicked
    /// </summary>
    public class ElementClickedEvent : EventArgs
    {
        public string Name { get; private set; }
        public ElementClickedEvent(string name)
        {
            Name = name;
        }
    }
    /// <summary>
    /// Event for when a new device connects
    /// </summary>
    public class DeviceArrivalEvent : EventArgs
    {
        public DeviceType DeviceType { get; private set; }
        public DeviceArrivalEvent(DeviceType type)
        {
            DeviceType = type;
        }
    }
    #endregion

    /// <summary>
    /// Orientation enumeration of a device
    /// </summary>
    public enum Orientation { Landscape, Portrait }
    /// <summary>
    /// Phone device types enumeration
    /// </summary>
    public enum DeviceType { iPhone, iPad, AndroidSmall, AndroidNormal, AndroidLarge, AndroidXL, AndroidOther }
    enum ARXError { Success, WrongParameterFormat, NullParameterNotSupported, WrongFilePath, SDKNotInitialized, SDKAlreadyInitialized, ConnectionWithLGSBroken, ErrorCreatingThread, ErrorCopyingMemory }

    /// <summary>
    /// ARX SDK for Logitech Keyboards
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class LogiARX
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        protected internal delegate void logiArxCB(int eventType, int eventValue, [MarshalAs(UnmanagedType.LPWStr)]string eventArg, IntPtr context);
        protected internal logiArxCbContext contextCallback;
        protected internal struct logiArxCbContext
        {
            internal logiArxCB arxCallBack;
            internal IntPtr arxContext;
        }

        #region Constants & Delegates
        protected const int LOGI_ARX_ORIENTATION_PORTRAIT = 0x01;
        protected const int LOGI_ARX_ORIENTATION_LANDSCAPE = 0x10;
        protected const int LOGI_ARX_EVENT_FOCUS_ACTIVE = 0x01;
        protected const int LOGI_ARX_EVENT_FOCUS_INACTIVE = 0x02;
        protected const int LOGI_ARX_EVENT_TAP_ON_TAG = 0x04;
        protected const int LOGI_ARX_EVENT_MOBILEDEVICE_ARRIVAL = 0x08;
        protected const int LOGI_ARX_EVENT_MOBILEDEVICE_REMOVAL = 0x10;
        protected const int LOGI_ARX_DEVICETYPE_IPHONE = 0x01;
        protected const int LOGI_ARX_DEVICETYPE_IPAD = 0x02;
        protected const int LOGI_ARX_DEVICETYPE_ANDROID_SMALL = 0x03;
        protected const int LOGI_ARX_DEVICETYPE_ANDROID_NORMAL = 0x04;
        protected const int LOGI_ARX_DEVICETYPE_ANDROID_LARGE = 0x05;
        protected const int LOGI_ARX_DEVICETYPE_ANDROID_XLARGE = 0x06;
        protected const int LOGI_ARX_DEVICETYPE_ANDROID_OTHER = 0x07;
        #endregion

        #region Events
        public virtual event EventHandler<LogEvent> ErrorMessage;
        protected virtual void OnError(string message) { throw new NotImplementedException(); }
        public virtual event EventHandler<ElementClickedEvent> ElementClicked;
        protected virtual void OnElementClicked(string elementName) { throw new NotImplementedException(); }

        public virtual event EventHandler FocusInactive;
        protected virtual void OnFocusInactive()
        {
            if (FocusInactive != null) FocusInactive(this, new EventArgs());
        }
        public virtual event EventHandler<OrientationEvent> FocusActive;
        protected virtual void OnFocusActive(Orientation orient) { throw new NotImplementedException(); }

        public virtual event EventHandler DeviceRemoval;
        protected virtual void OnDeviceRemoval() { throw new NotImplementedException(); }
        public virtual event EventHandler DeviceArrival;
        protected virtual void OnDeviceArrival(DeviceType type) { throw new NotImplementedException(); }
        #endregion

        #region Methods
        //Common methods. Inatialize, Shutdown etc.
        public virtual int LastErrorCode { get; set; }
        public virtual string LastError { get; set; }
        public virtual void Shutdown() { throw new NotImplementedException(); }
        //Files
        public virtual bool AddFile(string path) { throw new NotImplementedException(); }
        public virtual bool AddFile(string path, string name) { throw new NotImplementedException(); }
        public virtual bool AddAllFiles(string directory) { throw new NotImplementedException(); }
        //Other content
        public virtual bool AddContent(byte[] content, int size, string fileName) { throw new NotImplementedException(); }
        public virtual bool AddUTF8String(string stringContent, string fileName) { throw new NotImplementedException(); }
        public virtual bool AddImage(byte[] bitmap, int width, int height, string fileName) { throw new NotImplementedException(); }
        //HTML & page setters
        public virtual bool SetActivePage(string fileName) { throw new NotImplementedException(); }
        public virtual bool SetProperty(string id, string property, string value) { throw new NotImplementedException(); }
        public virtual bool SetContent(string id, string content) { throw new NotImplementedException(); }
        #endregion

        public bool Initialized { get; set; }

        public static LogiARX Load(string id, string name)
        {
            if (Environment.Is64BitProcess) //x64 Process
            {
                return new ARX64(id, name);
            }
            else //x86 Process
            {
                return new ARX86(id, name);
            }
        }
        protected LogiARX() { }

        //Used for Callback when user clicks something on the screen.
        protected void SDKCallback(int eventType, int eventValue, string eventArg, IntPtr context)
        {
            switch (eventType)
            {
                //The applet is now in background.
                case LOGI_ARX_EVENT_FOCUS_INACTIVE: OnFocusInactive(); break;

                //The applet has received focus and is now in active status.
                case LOGI_ARX_EVENT_FOCUS_ACTIVE:
                    {
                        if (eventValue == LOGI_ARX_ORIENTATION_PORTRAIT) OnFocusActive(Orientation.Portrait);
                        else if (eventValue == LOGI_ARX_ORIENTATION_LANDSCAPE) OnFocusActive(Orientation.Landscape);
                    }
                    break;

                //A mobile device is now connected.
                case LOGI_ARX_EVENT_MOBILEDEVICE_ARRIVAL:
                    {
                        switch (eventValue)
                        {
                            case LOGI_ARX_DEVICETYPE_IPHONE: OnDeviceArrival(DeviceType.iPhone); break;
                            case LOGI_ARX_DEVICETYPE_IPAD: OnDeviceArrival(DeviceType.iPad); break;
                            case LOGI_ARX_DEVICETYPE_ANDROID_SMALL: OnDeviceArrival(DeviceType.AndroidSmall); break;
                            case LOGI_ARX_DEVICETYPE_ANDROID_NORMAL: OnDeviceArrival(DeviceType.AndroidNormal); break;
                            case LOGI_ARX_DEVICETYPE_ANDROID_LARGE: OnDeviceArrival(DeviceType.AndroidLarge); break;
                            case LOGI_ARX_DEVICETYPE_ANDROID_XLARGE: OnDeviceArrival(DeviceType.AndroidXL); break;
                            case LOGI_ARX_DEVICETYPE_ANDROID_OTHER: OnDeviceArrival(DeviceType.AndroidOther); break;
                        }
                    }
                    break;

                //No more devices connected to Logitech Gaming Software.
                case LOGI_ARX_EVENT_MOBILEDEVICE_REMOVAL: OnDeviceRemoval(); break;

                //he user has tapped on an element in the applet HTML active page.
                case LOGI_ARX_EVENT_TAP_ON_TAG: OnElementClicked(eventArg); break;

                default: OnError("EVENT TRIGGERED"); break;
            }
        }
    }

    /// <summary>
    /// 64-bit version of the ARX SDK for Logitech Keyboards
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    class ARX64 : LogiARX
    {
        #region DLL
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxInit(string identifier, string friendlyName, ref logiArxCbContext callback);
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxAddFileAs(string filePath, string fileName, string mimeType = "");
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxAddContentAs(byte[] content, int size, string fileName, string mimeType = "");
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxAddUTF8StringAs(string stringContent, string fileName, string mimeType = "");
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxAddImageFromBitmap(byte[] bitmap, int width, int height, string fileName);
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxSetIndex(string fileName);
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxSetTagPropertyById(string tagId, string prop, string newValue);
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxSetTagsPropertyByClass(string tagsClass, string prop, string newValue);
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxSetTagContentById(string tagId, string newContent);
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxSetTagsContentByClass(string tagsClass, string newContent);
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern int LogiArxGetLastError();
        [DllImport("dll\\ARX64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern void LogiArxShutdown();
        #endregion

        #region Events
        public override event EventHandler<LogEvent> ErrorMessage;
        protected override void OnError(string message)
        {
            if (ErrorMessage != null) ErrorMessage(this, new LogEvent(message));
        }
        public override event EventHandler<ElementClickedEvent> ElementClicked;
        protected override void OnElementClicked(string elementName)
        {
            if (ElementClicked != null) ElementClicked(this, new ElementClickedEvent(elementName));
        }

        public override event EventHandler FocusInactive;
        protected override void OnFocusInactive()
        {
            if (FocusInactive != null) FocusInactive(this, new EventArgs());
        }
        public override event EventHandler<OrientationEvent> FocusActive;
        protected override void OnFocusActive(Orientation orient)
        {
            if (FocusActive != null) FocusActive(this, new OrientationEvent(orient));
        }

        public override event EventHandler DeviceRemoval;
        protected override void OnDeviceRemoval()
        {
            if (DeviceRemoval != null) DeviceRemoval(this, new EventArgs());
        }
        public override event EventHandler DeviceArrival;
        protected override void OnDeviceArrival(DeviceType type)
        {
            if (DeviceArrival != null) DeviceArrival(this, new DeviceArrivalEvent(type));
        }
        #endregion

        #region Methods
        //Common methods. Inatialize, Shutdown etc.
        bool Init(string identifier, string friendlyName, ref logiArxCbContext callback)
        {
            if (LogiArxInit(identifier, friendlyName, ref callback)) return true;
            else
            {
                OnError("ARX NOT INITIALIZED: " + LastError);
                return true;
            }
        }
        public override int LastErrorCode { get { return LogiArxGetLastError(); } }
        public override string LastError { get { return Enum.GetName(typeof(ARXError), LogiArxGetLastError()); } }
        public override void Shutdown()
        {
            LogiArxShutdown();
        }
        //Files
        public override bool AddFile(string path)
        {
            string fileName = path.Substring(path.LastIndexOf(@"\") + 1);
            if (LogiArxAddFileAs(path, fileName, "")) return true;
            else
            {
                OnError("FILE '" + fileName + "' NOT ADDED: " + LastError);
                return false;
            }
        }
        public override bool AddFile(string path, string name)
        {
            if (LogiArxAddFileAs(path, name, "")) return true;
            else
            {
                OnError("FILE '" + name + "' NOT ADDED: " + LastError);
                return false;
            }
        }
        public override bool AddAllFiles(string directory)
        {
            try
            {
                if (!directory.EndsWith("\\")) directory += "\\";

                foreach (string file in System.IO.Directory.GetFiles(directory))
                {
                    AddFile(file);
                }
                return true;
            }
            catch { return false; }
        }
        //Other content
        public override bool AddContent(byte[] content, int size, string fileName)
        {
            if (LogiArxAddContentAs(content, size, fileName, "")) return true;
            else
            {
                OnError("CONTENT '" + fileName + "' NOT ADDED: " + LastError);
                return false;
            }
        }
        public override bool AddUTF8String(string stringContent, string fileName)
        {
            if (LogiArxAddUTF8StringAs(stringContent, fileName, "")) return true;
            else
            {
                OnError("STRING '" + fileName + "' NOT ADDED: " + LastError);
                return false;
            }
        }
        public override bool AddImage(byte[] bitmap, int width, int height, string fileName)
        {
            if (LogiArxAddImageFromBitmap(bitmap, width, height, fileName)) return true;
            else
            {
                OnError("IMAGE '" + fileName + "' NOT ADDED: " + LastError);
                return false;
            }
        }
        //HTML & page setters
        public override bool SetActivePage(string fileName)
        {
            if (LogiArxSetIndex(fileName)) return true;
            else
            {
                OnError("ACTIVE INDEX ERROR " + fileName);
                return false;
            }
        }
        public override bool SetProperty(string id, string property, string value)
        {
            if (id.StartsWith(".")) return LogiArxSetTagsPropertyByClass(id.Substring(1), property, value);
            else if (id.StartsWith("#")) return LogiArxSetTagPropertyById(id.Substring(1), property, value);
            else { OnError("PROPERTY NOT SET: " + LastError); return false; }
        }
        public override bool SetContent(string id, string content)
        {
            if (id.StartsWith(".")) return LogiArxSetTagsContentByClass(id.Substring(1), content);
            else if (id.StartsWith("#")) return LogiArxSetTagContentById(id.Substring(1), content);
            else { OnError("CONTENT NOT SET: " + LastError); return false; }
        }
        #endregion

        //Constructor
        internal ARX64(string id, string name)
        {
            Initialized = false;
            contextCallback.arxCallBack = new logiArxCB(SDKCallback);
            contextCallback.arxContext = IntPtr.Zero;

            if (Init(id, name, ref contextCallback)) { OnError("Initialized"); Initialized = true; }
            else OnError(LastError);
        }
    }
    /// <summary>
    /// 32-bit version of the ARX SDK for Logitech Keyboards
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    class ARX86 : LogiARX
    {
        #region DLL
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxInit(string identifier, string friendlyName, ref logiArxCbContext callback);
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxAddFileAs(string filePath, string fileName, string mimeType = "");
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxAddContentAs(byte[] content, int size, string fileName, string mimeType = "");
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxAddUTF8StringAs(string stringContent, string fileName, string mimeType = "");
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxAddImageFromBitmap(byte[] bitmap, int width, int height, string fileName);
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxSetIndex(string fileName);
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxSetTagPropertyById(string tagId, string prop, string newValue);
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxSetTagsPropertyByClass(string tagsClass, string prop, string newValue);
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxSetTagContentById(string tagId, string newContent);
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiArxSetTagsContentByClass(string tagsClass, string newContent);
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern int LogiArxGetLastError();
        [DllImport("dll\\ARX86.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern void LogiArxShutdown();
        #endregion

        #region Events
        public override event EventHandler<LogEvent> ErrorMessage;
        protected override void OnError(string message)
        {
            if (ErrorMessage != null) ErrorMessage(this, new LogEvent(message));
        }
        public override event EventHandler<ElementClickedEvent> ElementClicked;
        protected override void OnElementClicked(string elementName)
        {
            if (ElementClicked != null) ElementClicked(this, new ElementClickedEvent(elementName));
        }

        public override event EventHandler FocusInactive;
        protected override void OnFocusInactive()
        {
            if (FocusInactive != null) FocusInactive(this, new EventArgs());
        }
        public override event EventHandler<OrientationEvent> FocusActive;
        protected override void OnFocusActive(Orientation orient)
        {
            if (FocusActive != null) FocusActive(this, new OrientationEvent(orient));
        }

        public override event EventHandler DeviceRemoval;
        protected override void OnDeviceRemoval()
        {
            if (DeviceRemoval != null) DeviceRemoval(this, new EventArgs());
        }
        public override event EventHandler DeviceArrival;
        protected override void OnDeviceArrival(DeviceType type)
        {
            if (DeviceArrival != null) DeviceArrival(this, new DeviceArrivalEvent(type));
        }
        #endregion

        #region Methods
        //Common methods. Inatialize, Shutdown etc.
        bool Init(string identifier, string friendlyName, ref logiArxCbContext callback)
        {
            if (LogiArxInit(identifier, friendlyName, ref callback)) return true;
            else
            {
                OnError("ARX NOT INITIALIZED: " + LastError);
                return true;
            }
        }
        public override int LastErrorCode { get { return LogiArxGetLastError(); } }
        public override string LastError { get { return Enum.GetName(typeof(ARXError), LogiArxGetLastError()); } }
        public override void Shutdown()
        {
            LogiArxShutdown();
        }
        //Files
        public override bool AddFile(string path)
        {
            string fileName = path.Substring(path.LastIndexOf(@"\") + 1);
            if (LogiArxAddFileAs(path, fileName, "")) return true;
            else
            {
                OnError("FILE '" + fileName + "' NOT ADDED: " + LastError);
                return false;
            }
        }
        public override bool AddFile(string path, string name)
        {
            if (LogiArxAddFileAs(path, name, "")) return true;
            else
            {
                OnError("FILE '" + name + "' NOT ADDED: " + LastError);
                return false;
            }
        }
        public override bool AddAllFiles(string directory)
        {
            try
            {
                if (!directory.EndsWith("\\")) directory += "\\";

                foreach (string file in System.IO.Directory.GetFiles(directory))
                {
                    AddFile(file);
                }
                return true;
            }
            catch { return false; }
        }
        //Other content
        public override bool AddContent(byte[] content, int size, string fileName)
        {
            if (LogiArxAddContentAs(content, size, fileName, "")) return true;
            else
            {
                OnError("CONTENT '" + fileName + "' NOT ADDED: " + LastError);
                return false;
            }
        }
        public override bool AddUTF8String(string stringContent, string fileName)
        {
            if (LogiArxAddUTF8StringAs(stringContent, fileName, "")) return true;
            else
            {
                OnError("STRING '" + fileName + "' NOT ADDED: " + LastError);
                return false;
            }
        }
        public override bool AddImage(byte[] bitmap, int width, int height, string fileName)
        {
            if (LogiArxAddImageFromBitmap(bitmap, width, height, fileName)) return true;
            else
            {
                OnError("IMAGE '" + fileName + "' NOT ADDED: " + LastError);
                return false;
            }
        }
        //HTML & page setters
        public override bool SetActivePage(string fileName)
        {
            if (LogiArxSetIndex(fileName)) return true;
            else
            {
                OnError("ACTIVE INDEX ERROR " + fileName);
                return false;
            }
        }
        public override bool SetProperty(string id, string property, string value)
        {
            if (id.StartsWith(".")) return LogiArxSetTagsPropertyByClass(id.Substring(1), property, value);
            else if (id.StartsWith("#")) return LogiArxSetTagPropertyById(id.Substring(1), property, value);
            else { OnError("PROPERTY NOT SET: " + LastError); return false; }
        }
        public override bool SetContent(string id, string content)
        {
            if (id.StartsWith(".")) return LogiArxSetTagsContentByClass(id.Substring(1), content);
            else if (id.StartsWith("#")) return LogiArxSetTagContentById(id.Substring(1), content);
            else { OnError("CONTENT NOT SET: " + LastError); return false; }
        }
        #endregion

        //Constructor
        internal ARX86(string id, string name)
        {
            Initialized = false;
            contextCallback.arxCallBack = new logiArxCB(SDKCallback);
            contextCallback.arxContext = IntPtr.Zero;

            if (Init(id, name, ref contextCallback)) { OnError("Initialized"); Initialized = true; }
            else OnError(LastError);
        }
    }
}
