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
            if (_config == null) return new DesktopUIAdapter();

            return _config.selectedPlatform switch
            {
                UIConfiguration.TargetPlatform.VR => new VRUIAdapter(),
                UIConfiguration.TargetPlatform.Mobile => new MobileUIAdapter(),
                _ => new DesktopUIAdapter()
            };
        }
    }
}