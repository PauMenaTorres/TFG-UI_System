using UnityEngine;

namespace ModularUIRuntime
{
    public class PlatformFactory : IPlatformFactory
    {
        private readonly UIConfiguration _config;

        public PlatformFactory(UIConfiguration config)
        {
            _config = config;
        }

        public IPlatformUIAdapter CreateAdapter()
        {
            if (_config == null)
            {
                return new DesktopUIAdapter();
            }

            return _config.selectedPlatform switch
            {
                UIConfiguration.TargetPlatform.VR => new VRUIAdapter(),
                UIConfiguration.TargetPlatform.MobilePortrait => new MobileUIAdapter(new Vector2(1080, 1920), 0f),
                UIConfiguration.TargetPlatform.MobileLandscape => new MobileUIAdapter(new Vector2(1920, 1080), 1f),
                _ => new DesktopUIAdapter()
            };
        }
    }
}