using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
    [FilePath( "UserSettings/Kogane/DeviceSimulatorBookmarkSetting.asset", FilePathAttribute.Location.ProjectFolder )]
    internal sealed class DeviceSimulatorBookmarkSetting : ScriptableSingleton<DeviceSimulatorBookmarkSetting>
    {
        [SerializeField] private List<string> m_deviceNames = new();

        public IReadOnlyList<string> DeviceNames => m_deviceNames;

        public void Save()
        {
            Save( true );
        }

        public bool Contains( string deviceName )
        {
            return m_deviceNames.Contains( deviceName );
        }

        public void Add( string deviceName )
        {
            m_deviceNames.Add( deviceName );
            Save();
        }
    }
}