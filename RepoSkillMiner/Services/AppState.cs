using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoSkillMiner.Services
{
    public class AppState
    {
        public string Displayurl { get; private set; }

        public event Action OnChange;

        public void SetDisplayUrl(string url)
        {
            Displayurl = url;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
