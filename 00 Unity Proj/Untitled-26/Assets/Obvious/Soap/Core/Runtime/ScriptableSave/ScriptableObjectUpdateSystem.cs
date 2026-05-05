using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Obvious.Soap
{
    public static class ScriptableObjectUpdateSystem
    {
        private static List<ScriptableSaveBase> _objectsToUpdate = new List<ScriptableSaveBase>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var updateSystemIndex = Array.FindIndex(playerLoop.subSystemList, x => x.type == typeof(Update));

            if (updateSystemIndex != -1)
            {
                var updateSystem = playerLoop.subSystemList[updateSystemIndex];
                var oldList = updateSystem.subSystemList.ToList();
                oldList.Add(new PlayerLoopSystem
                {
                    type = typeof(ScriptableObjectUpdateSystem),
                    updateDelegate = UpdateScriptableObjects
                });
                updateSystem.subSystemList = oldList.ToArray();
                playerLoop.subSystemList[updateSystemIndex] = updateSystem;
                PlayerLoop.SetPlayerLoop(playerLoop);
            }
        }

        public static void RegisterObject(ScriptableSaveBase scriptableSave)
        {
            if (!_objectsToUpdate.Contains(scriptableSave))
            {
                _objectsToUpdate.Add(scriptableSave);
            }
        }

        public static void UnregisterObject(ScriptableSaveBase scriptableSave)
        {
            _objectsToUpdate.Remove(scriptableSave);
        }

        private static void UpdateScriptableObjects()
        {
            foreach (var savableScriptableObject in _objectsToUpdate)
            {
                savableScriptableObject.Update();
            }
        }
    }
}