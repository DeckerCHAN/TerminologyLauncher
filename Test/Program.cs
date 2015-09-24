using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;
using TerminologyLauncher.I18n.TranslationObjects;
using TerminologyLauncher.I18n.TranslationObjects.GUITranslations;
using TerminologyLauncher.I18n.TranslationObjects.HandlerTranslations;

namespace TerminologyLauncher.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var translation = new TranslationRoot()
            {
                GuiTranslation = new GUITranslationRoot()
                {
                    LoginWindowTranslation = new LoginWindowTranslation
                    {
                        LoginWindowTitleTranslation = "Terminology Login",
                        CancelTranslation = "Cancel",
                        LoginTranslation = "Login",
                        MojongAccountTranslation = "Mojong Account:",
                        OfflineAccountTranslation = "Offline Account:",
                        LoginModeTranslation = "Login Mode:",
                        OfficialModeTranslation = "Official Login",
                        OfflineModeTranslation = "Offline Login",
                        PasswordTranslation = "Password:",
                        RememberAccountTranslation = "Remember account",
                        LoginFaultTranslation = "Can not login!",
                        LoginFaultInsufficientArgumentsTranslation = "Insufficient Arguments",
                        LoginFaultWrongPasswordTranslation = "Wrong password!",
                        LoginFaultUserNotExistTranslation = "User not exists!",
                        LoginFaultNetworkTimedOutTranslation = "Network timed out",
                        LoginFaultUnknownErrorTranslation = "Unknown error"
                    },
                    SingleSelectWindowTranslation = new SingleSelectWindowTranslation()
                    {
                        ConfirmTranslation = "Confirm"
                    },
                    MainWindowTranslation = new MainWindowTranslation()
                    {
                        AddInstanceTranslation = "New instance",
                        RemoveInstanceTranslation = "Remove select",
                        LaunchInstanceTranslation = "Launch me!",
                        UpdateInstanceTranslation = "Update me!",
                        InstanceAuthorFieldTranslation = "Author:",
                        InstanceDescribeFieldTranslation = "Describe:",
                        InstanceNameFieldTranslation = "Name:",
                        InstanceVersionFieldTranslation = "Version:",
                        MainWindowTitleTranslation = "Terminology Launcher",
                        OfficialLoginTranslation = "Official Mode",
                        OfflineLoginTranslation = "Offline Mode"
                    }
                },
                HandlerTranslation = new HandlerTranslationRoot()
                {
                    JavaSelectTranslation = new JavaSelectTranslation()
                    {
                        JavaSelectFieldTranslation = "Java Runtime:",
                        JavaSelectWindowTitleTranslation = "Select a JRE"
                    }
                }
            };
            File.WriteAllText("G:\\CSharp\\TerminologyLauncher\\I18n\\Translations\\en_us", JsonConverter.ConvertToJson(translation),Encoding.UTF8);
            Console.ReadKey();
        }
    }
}
