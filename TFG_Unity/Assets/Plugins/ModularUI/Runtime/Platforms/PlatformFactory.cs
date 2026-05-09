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
            Vector2 baseRes = new Vector2(800, 600);

            if (_config != null)
            {
                baseRes = _config.designResolution;
            }

            if (_config == null)
            {
                return new DesktopUIAdapter(baseRes, 0.5f);
            }

            return _config.selectedPlatform switch
            {
                UIConfiguration.TargetPlatform.VR => new VRUIAdapter(),
                UIConfiguration.TargetPlatform.MobilePortrait => new MobileUIAdapter(new Vector2(baseRes.y, baseRes.x), 1f),
                UIConfiguration.TargetPlatform.MobileLandscape => new MobileUIAdapter(baseRes, 0f),
                _ => new DesktopUIAdapter(baseRes, 0.5f)
            };
        }
    }
}