namespace Key.Scripts.Pooling {
    public interface IPoolable {
        public void OnGetFromPool();
        public void OnReturnToPool();
    }
}