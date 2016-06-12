using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CreviceApp.User
{
    using static WinAPI.SendInput.Constant.VirtualKeys;

    class UserConfig : Core.Config.UserConfig
    {
        public UserConfig()
        {
            var Chrome = @when((ctx) =>
            {
                return ctx.Window.Foreground.Module.Name == "chrome.exe";
            });

            Chrome.
            @on(RightButton).
            @if(WheelUp).
            @do((ctx) =>
            {
                SendInput.Multiple().
                ExtendedKeyDown(VK_CONTROL).
                ExtendedKeyDown(VK_SHIFT).
                ExtendedKeyDown(VK_TAB).
                ExtendedKeyUp(VK_TAB).
                ExtendedKeyUp(VK_SHIFT).
                ExtendedKeyUp(VK_CONTROL).
                Send();

                Tooltip("Previous Tab");
            });

            Chrome.
            @on(RightButton).
            @if(WheelDown).
            @do((ctx) =>
            {
                SendInput.Multiple().
                ExtendedKeyDown(VK_CONTROL).
                ExtendedKeyDown(VK_TAB).
                ExtendedKeyUp(VK_TAB).
                ExtendedKeyUp(VK_CONTROL).
                Send();
            });

            Chrome.
            @on(RightButton).
            @if(MoveUp).
            @do((ctx) =>
            {
                SendInput.Multiple().
                ExtendedKeyDown(VK_HOME).
                ExtendedKeyUp(VK_HOME).
                Send();
            });

            Chrome.
            @on(RightButton).
            @if(MoveDown).
            @do((ctx) =>
            {
                SendInput.Multiple().
                ExtendedKeyDown(VK_END).
                ExtendedKeyUp(VK_END).
                Send();
            });

            Chrome.
            @on(RightButton).
            @if(MoveDown, MoveRight).
            @do((ctx) =>
            {
                SendInput.Multiple().
                ExtendedKeyDown(VK_CONTROL).
                ExtendedKeyDown(VK_W).
                ExtendedKeyUp(VK_W).
                ExtendedKeyUp(VK_CONTROL).
                Send();
            });
        }
    }
}
