using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kogane.Internal
{
    [UsedImplicitly]
    internal sealed class DeviceSimulatorBookmarkPlugin : DeviceSimulatorPlugin
    {
        private static readonly Type      SIMULATOR_WINDOW_TYPE       = typeof( SimulatorWindow );
        private static readonly FieldInfo DEVICE_SIMULATOR_MAIN_FIELD = SIMULATOR_WINDOW_TYPE.GetField( "m_Main", BindingFlags.Instance | BindingFlags.NonPublic );

        public override string title => "Bookmark";

        public override VisualElement OnCreateUI()
        {
            var root = new VisualElement();

            foreach ( var deviceName in DeviceSimulatorBookmarkSetting.instance.DeviceNames )
            {
                var setDeviceButton = new Button( () => SetDeviceIndex( deviceName ) )
                {
                    text = deviceName,
                };

                root.Add( setDeviceButton );
            }

            var addBookmarkButton = new Button( () => AddBookmark() )
            {
                text = "Add Bookmark",
            };

            var refreshButton = new Button( () => Refresh() )
            {
                text = "Refresh",
            };

            var projectSettingsButton = new Button( () => SettingsService.OpenProjectSettings( DeviceSimulatorBookmarkSettingProvider.PATH ) )
            {
                text = "Project Settings",
            };

            root.Add( addBookmarkButton );
            root.Add( refreshButton );
            root.Add( projectSettingsButton );

            return root;
        }

        private static void SetDeviceIndex( string deviceName )
        {
            var simulatorWindow = Resources
                    .FindObjectsOfTypeAll<SimulatorWindow>()
                    .FirstOrDefault()
                ;

            var deviceSimulatorMain = ( DeviceSimulatorMain )DEVICE_SIMULATOR_MAIN_FIELD.GetValue( simulatorWindow );
            var devices             = deviceSimulatorMain.devices;
            var deviceIndex         = Array.FindIndex( devices, x => x.deviceInfo.friendlyName == deviceName );

            if ( deviceIndex == -1 ) return;

            deviceSimulatorMain.deviceIndex = deviceIndex;
        }

        private static void AddBookmark()
        {
            var simulatorWindow = Resources
                    .FindObjectsOfTypeAll<SimulatorWindow>()
                    .FirstOrDefault()
                ;

            var deviceSimulatorMain = ( DeviceSimulatorMain )DEVICE_SIMULATOR_MAIN_FIELD.GetValue( simulatorWindow );
            var currentDevice       = deviceSimulatorMain.currentDevice;
            var friendlyName        = currentDevice.deviceInfo.friendlyName;

            var setting = DeviceSimulatorBookmarkSetting.instance;

            if ( setting.Contains( friendlyName ) ) return;

            setting.Add( friendlyName );

            Refresh();
        }

        private static void Refresh()
        {
            var simulatorWindow = Resources
                    .FindObjectsOfTypeAll<SimulatorWindow>()
                    .FirstOrDefault()
                ;

            var editorAssembly           = typeof( Editor ).Assembly;
            var simulatorWindowType      = simulatorWindow.GetType();
            var gameViewType             = editorAssembly.GetType( "UnityEditor.GameView" );
            var playModeViewType         = editorAssembly.GetType( "UnityEditor.PlayModeView" );
            var swapMainWindowMethodInfo = playModeViewType.GetMethod( "SwapMainWindow", BindingFlags.Instance | BindingFlags.NonPublic );

            swapMainWindowMethodInfo.Invoke( simulatorWindow, new[] { gameViewType } );

            var gameWindow = Resources
                    .FindObjectsOfTypeAll( gameViewType )
                    .FirstOrDefault()
                ;

            swapMainWindowMethodInfo.Invoke( gameWindow, new[] { simulatorWindowType } );
        }
    }
}