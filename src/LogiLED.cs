using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System;

using System.Collections.Generic;
using System.Windows.Media;

namespace LogiSDK
{
    #region Enums & Classes
    /// <summary>
    /// Used to create a color for the Logitech LED SDK
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class LedColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public LedColor() { }
        public LedColor(byte R, byte G, byte B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public bool Equals(LedColor other)
        {
            if (R == other.R && G == other.G && B == other.B) return true;
            else return false;
        }

        internal int Rperc { get { return (int)Math.Round(R / 2.55, MidpointRounding.ToEven); } }
        internal int Gperc { get { return (int)Math.Round(G / 2.55, MidpointRounding.ToEven); } }
        internal int Bperc { get { return (int)Math.Round(B / 2.55, MidpointRounding.ToEven); } }

        public string ToHex()
        {
            return "#FF" + R.ToString("X") + G.ToString("X") + B.ToString("X");
        }
    }
    /// <summary>
    /// Predefined colors for the Logitech LED SDK
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class LedColors
    {
        public static LedColor White { get { return new LedColor(255, 255, 255); } }
        public static LedColor Silver { get { return new LedColor(192, 192, 192); } }
        public static LedColor Gray { get { return new LedColor(128, 128, 128); } }
        public static LedColor Black { get { return new LedColor(0, 0, 0); } }
        public static LedColor Red { get { return new LedColor(255, 0, 0); } }
        public static LedColor Maroon { get { return new LedColor(128, 0, 0); } }
        public static LedColor Yellow { get { return new LedColor(255, 255, 0); } }
        public static LedColor Olive { get { return new LedColor(128, 128, 255); } }
        public static LedColor Lime { get { return new LedColor(0, 255, 0); } }
        public static LedColor Green { get { return new LedColor(0, 128, 0); } }
        public static LedColor Aqua { get { return new LedColor(0, 255, 255); } }
        public static LedColor Teal { get { return new LedColor(0, 128, 128); } }
        public static LedColor Blue { get { return new LedColor(0, 0, 255); } }
        public static LedColor Navy { get { return new LedColor(0, 0, 128); } }
        public static LedColor Magenta { get { return new LedColor(255, 0, 255); } }
        public static LedColor Purple { get { return new LedColor(128, 0, 128); } }
    }
    /// <summary>
    /// Key enumeration of the keys available on a keyboard
    /// </summary>
    public enum LedKey
    {
        A = 4,
        B = 5,
        C = 6,
        D = 7,
        E = 8,
        F = 9,
        G = 10,
        H = 11,
        I = 12,
        J = 13,
        K = 14,
        L = 15,
        M = 16,
        N = 17,
        O = 18,
        P = 19,
        Q = 20,
        R = 21,
        S = 22,
        T = 23,
        U = 24,
        V = 25,
        W = 26,
        X = 27,
        Y = 28,
        Z = 29,
        D1 = 30,
        D2 = 31,
        D3 = 32,
        D4 = 33,
        D5 = 34,
        D6 = 35,
        D7 = 36,
        D8 = 37,
        D9 = 38,
        D0 = 39,
        Return = 40,
        Escape = 41,
        Back = 42,
        Tab = 43,
        Space = 44,
        Oemplus = 45,
        Oem4 = 46,
        Oem6 = 47,
        OemSemicolon = 48,

        Oem2 = 50,
        Oemtilde = 51,
        OemQuotes = 52,
        OemPipe = 53,
        Oemcomma = 54,
        OemPeriod = 55,
        OemMinus = 56,
        CapsLock = 57,
        F1 = 58,
        F2 = 59,
        F3 = 60,
        F4 = 61,
        F5 = 62,
        F6 = 63,
        F7 = 64,
        F8 = 65,
        F9 = 66,
        F10 = 67,
        F11 = 68,
        F12 = 69,
        PrintScreen = 70,
        Scroll = 71,
        Pause = 72,
        Insert = 73,
        Home = 74,
        Prior = 75,
        Delete = 76,
        End = 77,
        PageDown = 78,
        Right = 79,
        Left = 80,
        Down = 81,
        Up = 82,
        NumLock = 83,
        Divide = 84,
        Multiply = 85,
        Subtract = 86,
        Add = 87,
        Enter = 88,
        NumPad1 = 89,
        NumPad2 = 90,
        NumPad3 = 91,
        NumPad4 = 92,
        NumPad5 = 93,
        NumPad6 = 94,
        NumPad7 = 95,
        NumPad8 = 96,
        NumPad9 = 97,
        NumPad0 = 98,
        Decimal = 99,
        Oem102 = 100,
        Apps = 101,

        LControlKey = 224,
        LShiftKey = 225,
        LMenu = 226,
        LWin = 227,
        RControlKey = 228,
        RShiftKey = 229,
        RMenu = 230,
        RWin = 231,

    }
    #endregion

    /// <summary>
    /// LED SDK for Logitech Keyboards
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class LogiLED
    {
        #region Constants
        protected const int LOGI_LED_BITMAP_WIDTH = 21;
        protected const int LOGI_LED_BITMAP_HEIGHT = 6;
        protected const int LOGI_LED_BITMAP_BYTES_PER_KEY = 4;
        protected const int LOGI_LED_BITMAP_SIZE = LOGI_LED_BITMAP_WIDTH * LOGI_LED_BITMAP_HEIGHT * LOGI_LED_BITMAP_BYTES_PER_KEY;
        protected const int LOGI_LED_DURATION_INFINITE = 0;
        #endregion

        /// <summary>
        /// Initialises Logitech LED for G910 'Orion Spark' ONLY. Also saves the current lighting.
        /// </summary>
        public static LogiLED Load()
        {
            if (Environment.Is64BitProcess) //x64 Process
            {
                return new LED64();
            }
            else //x86 Process
            {
                return new LED86();
            }
        }
        /// <summary>
        /// Forces initialization of the 64-bit version of the LED SDK
        /// </summary>
        public static LogiLED Force64()
        {
            return new LED64();
        }
        /// <summary>
        /// Forces initialization of the 32-bit version of the LED SDK
        /// </summary>
        public static LogiLED Force86()
        {
            return new LED86();
        }

        //Prevents usage of this class only LED64 or LED86
        protected LogiLED() { }

        public virtual void Shutdown() { throw new NotImplementedException(); }
        public virtual bool SaveCurrentLighting() { throw new NotImplementedException(); }
        public virtual bool RestoreLighting() { throw new NotImplementedException(); }

        public virtual bool StopEffects() { throw new NotImplementedException(); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">The RGB color</param>
        /// <param name="duration">The duration in milliseconds</param>
        /// <param name="interval">The interval in milliseconds</param>
        public virtual bool Pulse(LedColor color, int duration, int interval) { throw new NotImplementedException(); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">The RGB color</param>
        /// <param name="duration">The duration in milliseconds</param>
        /// <param name="interval">The interval in milliseconds</param>
        public virtual bool Flash(LedColor color, int duration, int interval) { throw new NotImplementedException(); }
        public virtual bool LightingFromBitmap(byte[] bitmap) { throw new NotImplementedException(); }

        public virtual bool SetKeyColor(LedKey key, LedColor color) { throw new NotImplementedException(); }
        public virtual bool SetKeyboardColor(LedColor color) { throw new NotImplementedException(); }
    }

    /// <summary>
    /// 64-bit version of the LED SDK for Logitech Keyboards
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class LED64 : LogiLED
    {
        #region Interop
        [DllImport("dll\\LED64.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedInit();

        [DllImport("dll\\LED64.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSaveCurrentLighting();

        [DllImport("dll\\LED64.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLighting(int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("dll\\LED64.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedRestoreLighting();

        [DllImport("dll\\LED64.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedFlashLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        [DllImport("dll\\LED64.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedPulseLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        [DllImport("dll\\LED64.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedStopEffects();

        [DllImport("dll\\LED64.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLightingFromBitmap(byte[] bitmap);

        [DllImport("dll\\LED64.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLightingForKeyWithScanCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("dll\\LED64.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLightingForKeyWithHidCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("dll\\LED64.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLightingForKeyWithQuartzCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("dll\\LED64.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLightingForKeyWithKeyNameCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("dll\\LED64.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern void LogiLedShutdown();
        #endregion

        public LED64()
        {
            LogiLedInit();
            SaveCurrentLighting();
        }

        public override void Shutdown() { LogiLedShutdown(); }
        public override bool SaveCurrentLighting() { return LogiLedSaveCurrentLighting(); }
        public override bool RestoreLighting() { return LogiLedRestoreLighting(); }

        public override bool StopEffects() { return LogiLedStopEffects(); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">The RGB color</param>
        /// <param name="duration">The duration in milliseconds</param>
        /// <param name="interval">The interval in milliseconds</param>
        public override bool Pulse(LedColor color, int duration, int interval) { return LogiLedPulseLighting(color.Rperc, color.Gperc, color.Bperc, duration, interval); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">The RGB color</param>
        /// <param name="duration">The duration in milliseconds</param>
        /// <param name="interval">The interval in milliseconds</param>
        public override bool Flash(LedColor color, int duration, int interval) { return LogiLedFlashLighting(color.Rperc, color.Gperc, color.Bperc, duration, interval); }
        public override bool LightingFromBitmap(byte[] bitmap) { return LogiLedSetLightingFromBitmap(bitmap); }

        public override bool SetKeyColor(LedKey key, LedColor color)
        {
            return LogiLedSetLightingForKeyWithHidCode((int)key, color.Rperc, color.Gperc, color.Bperc);
        }
        public override bool SetKeyboardColor(LedColor color)
        {
            return LogiLedSetLighting(color.Rperc, color.Gperc, color.Bperc);
        }
    }
    /// <summary>
    /// 32-bit version of the LED SDK for Logitech Keyboards
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class LED86 : LogiLED
    {
        #region Interop
        [DllImport("dll\\LED86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedInit();

        [DllImport("dll\\LED86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSaveCurrentLighting();

        [DllImport("dll\\LED86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLighting(int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("dll\\LED86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedRestoreLighting();

        [DllImport("dll\\LED86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedFlashLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        [DllImport("dll\\LED86.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedPulseLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        [DllImport("dll\\LED86.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedStopEffects();

        [DllImport("dll\\LED86.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLightingFromBitmap(byte[] bitmap);

        [DllImport("dll\\LED86.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLightingForKeyWithScanCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("dll\\LED86.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLightingForKeyWithHidCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("dll\\LED86.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLightingForKeyWithQuartzCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("dll\\LED86.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern bool LogiLedSetLightingForKeyWithKeyNameCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("dll\\LED86.dll ", CallingConvention = CallingConvention.Cdecl)]
        static extern void LogiLedShutdown();
        #endregion

        public LED86()
        {
            LogiLedInit();
            SaveCurrentLighting();
        }

        public override void Shutdown() { LogiLedShutdown(); }
        public override bool SaveCurrentLighting() { return LogiLedSaveCurrentLighting(); }
        public override bool RestoreLighting() { return LogiLedRestoreLighting(); }

        public override bool StopEffects() { return LogiLedStopEffects(); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">The RGB color</param>
        /// <param name="duration">The duration in milliseconds</param>
        /// <param name="interval">The interval in milliseconds</param>
        public override bool Pulse(LedColor color, int duration, int interval) { return LogiLedPulseLighting(color.Rperc, color.Gperc, color.Bperc, duration, interval); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">The RGB color</param>
        /// <param name="duration">The duration in milliseconds</param>
        /// <param name="interval">The interval in milliseconds</param>
        public override bool Flash(LedColor color, int duration, int interval) { return LogiLedFlashLighting(color.Rperc, color.Gperc, color.Bperc, duration, interval); }
        public override bool LightingFromBitmap(byte[] bitmap) { return LogiLedSetLightingFromBitmap(bitmap); }

        public override bool SetKeyColor(LedKey key, LedColor color)
        {
            return LogiLedSetLightingForKeyWithHidCode((int)key, color.Rperc, color.Gperc, color.Bperc);
        }
        public override bool SetKeyboardColor(LedColor color)
        {
            return LogiLedSetLighting(color.Rperc, color.Gperc, color.Bperc);
        }
    }
}