﻿using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Splines;

namespace Ltg8
{
    public abstract class OverworldEvent : OverworldBehavior
    {
        [SerializeField] 
        protected GameObject[] npcs;

        private Vector3[] _npcPositions;
        
        public bool IsDone { get; private set; }

        public override void OnStartOverworld()
        {
            base.OnStartOverworld();
            _npcPositions = new Vector3[npcs.Length];
            for (int i = 0; i < npcs.Length; ++i)
            {
                _npcPositions[i] = npcs[i].transform.position;
                npcs[i].transform.position = Game.Overworld.Pathing.HillsStartPosition;
            }
            RunLogic().Forget();
        }

        protected abstract UniTask RunLogic();

        protected static async UniTask Delay(float seconds)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds));
        }

        protected void NpcSay(int id, string message, float autoClearSeconds = -1)
        {
            npcs[id].SayText(message, autoClearSeconds);
        }

        protected async UniTask NpcMove(int id, PathType path)
        {
            GameObject npc = npcs[id];
            Spline spline = path switch {
                PathType.HillsToEntrance => Game.Overworld.Pathing.CreatePathBetween(path, npc.transform.position, _npcPositions[id]),
                PathType.EntranceToHills => Game.Overworld.Pathing.CreatePathBetween(path, _npcPositions[id], Game.Overworld.Pathing.HillsStartPosition),
                PathType.EntranceToCity => Game.Overworld.Pathing.CreatePathBetween(path, _npcPositions[id], Game.Overworld.Pathing.CityEndPosition),
                _ => throw new ArgumentOutOfRangeException(nameof(path), path, null)
            };
            await npc.FollowPath(spline);
        }

        protected async UniTask NpcMoveAll(PathType path)
        {
            UniTask[] movementTasks = new UniTask[npcs.Length];
            
            for (int i = 0; i < npcs.Length; i++)
                movementTasks[i] = NpcMove(i, path);

            await UniTask.WhenAll(movementTasks);
        }

        protected void FinishEvent()
        {
            IsDone = true;
        }
    }
}
