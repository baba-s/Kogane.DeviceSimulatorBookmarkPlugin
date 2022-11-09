using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.DeviceSimulation;
using UnityEngine.UIElements;

namespace Kogane.Internal
{
    [UsedImplicitly]
    internal sealed class DeviceSimulatorBookmarkPlugin : DeviceSimulatorPlugin
    {
        public override string title => "Bookmark";

        public override VisualElement OnCreateUI()
        {
            var root = new VisualElement();

            foreach ( var deviceName in DeviceSimulatorBookmarkSetting.instance.DeviceNames )
            {
                var setDeviceButton = new Button( () => DeviceSimulatorInternal.SetDeviceIndexFromDeviceName( deviceName ) )
                {
                    text = deviceName,
                };

                root.Add( setDeviceButton );
            }

            var addBookmarkButton = new Button( () => AddBookmark() )
            {
                text = "Add Bookmark",
            };

            var refreshButton = new Button( () => DeviceSimulatorInternal.Refresh() )
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

        private static void AddBookmark()
        {
            var friendlyName = DeviceSimulatorInternal.GetCurrentDeviceName();

            var setting = DeviceSimulatorBookmarkSetting.instance;

            if ( setting.Contains( friendlyName ) ) return;

            setting.Add( friendlyName );

            DeviceSimulatorInternal.Refresh();
        }
    }
}