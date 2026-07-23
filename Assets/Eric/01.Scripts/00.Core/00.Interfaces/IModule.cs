namespace Eric.ModuleSystem
{
        public interface IModule
        {
                void Init(ModuleOwner owner);
                void AfterInit();
        }
}