using UnityEngine;

namespace Obvious.Soap.Example
{
    [CreateAssetMenu(menuName = "Soap/Examples/ScriptableLists/Player")]
    public class ScriptableListPlayer : ScriptableList<Player>
    {
        //you can add custom methods in your custom scriptable lists like these!
        public void DestroyFirst()
        {
            var playerToDestroy = _list[0];
            Destroy(playerToDestroy.gameObject);
        }
    }
    
    [System.Serializable]
    public class ScriptableListPlayerReadOnly : ScriptableListReadOnly<ScriptableListPlayer, Player>
    {
    }
}