namespace TowerDefence
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SwarmDataValueContainer : ICloneable
    {
        public event System.EventHandler<SwarmDataValueContainer> e_ValuesHaveChanged;

        public List<S_Wave> Waves = new List<S_Wave>();
        public List<float> DefaultEnemyCooldowns = new List<float>();
        public List<int> DefaultWaveCooldowns = new List<int>();

        public SwarmDataValueContainer() { }

        public SwarmDataValueContainer(List<S_Wave> waves, List<float> defaultEnemyCooldowns, List<int> defaultWaveCooldowns)
        {
            Waves = new List<S_Wave>();
            for (int i = 0; i < waves.Count; i++) Waves.Add((S_Wave)waves[i].Clone());

            if (defaultEnemyCooldowns != null) DefaultEnemyCooldowns = defaultEnemyCooldowns;
            if (defaultWaveCooldowns != null) DefaultWaveCooldowns = defaultWaveCooldowns;
        }

        public void SetSwarmValues(SwarmDataValueContainer setTo, bool invokeEvent = true)
        {
            if (setTo.Waves == null || setTo.Waves.Count <= 0) return;

            Waves = new List<S_Wave>();
            for (int i = 0; i < setTo.Waves.Count; i++) Waves.Add((S_Wave)setTo.Waves[i].Clone());

            DefaultEnemyCooldowns = setTo.DefaultEnemyCooldowns;
            DefaultWaveCooldowns = setTo.DefaultWaveCooldowns;

            if (DefaultEnemyCooldowns == null || DefaultEnemyCooldowns.Count <= 0) DefaultEnemyCooldowns = new List<float>() { 0 };
            if (DefaultWaveCooldowns == null || DefaultWaveCooldowns.Count <= 0) DefaultWaveCooldowns = new List<int>() { 0 };

            //repeat last value untill sizes are the same
            int waveCount = Waves.Count;

            int diff = waveCount - DefaultEnemyCooldowns.Count;
            for (int i = 0; i < diff; i++) DefaultEnemyCooldowns.Add(DefaultEnemyCooldowns[DefaultEnemyCooldowns.Count - 1]);
            DefaultEnemyCooldowns = DefaultEnemyCooldowns.GetRange(0, waveCount);

            diff = waveCount - DefaultWaveCooldowns.Count;
            for (int i = 0; i < diff; i++) DefaultWaveCooldowns.Add(DefaultWaveCooldowns[DefaultWaveCooldowns.Count - 1]);
            DefaultWaveCooldowns = DefaultWaveCooldowns.GetRange(0, waveCount);

            if (invokeEvent) e_ValuesHaveChanged?.Invoke(this, this);
        }

        //public void InsertSwarmValues(SwarmDataValueContainer insertion, int insertAt)
        //{
        //    if (insertion == null) return;

        //    Refresh();
        //    insertion.Refresh();
        //    int oldCount = Waves.Count;

        //    if (insertAt >= oldCount || insertAt < 0)
        //    {
        //        Waves.AddRange(insertion.Waves);
        //        DefaultEnemyCooldowns.AddRange(insertion.DefaultEnemyCooldowns);
        //        DefaultWaveCooldowns.AddRange(insertion.DefaultWaveCooldowns);
        //    }
        //    else
        //    {
        //        Waves.InsertRange(insertAt, insertion.Waves);
        //        DefaultEnemyCooldowns.InsertRange(insertAt, insertion.DefaultEnemyCooldowns);
        //        DefaultWaveCooldowns.InsertRange(insertAt, insertion.DefaultWaveCooldowns);
        //    }
        //}

        public void Refresh() => SetSwarmValues(this, false);

        public object Clone()
        {
            var sdvc = this.MemberwiseClone() as SwarmDataValueContainer;
            List<S_Wave> waves = sdvc.Waves;
            sdvc.Waves = new List<S_Wave>();
            for (int i = 0; i < waves.Count; i++)
            {
                sdvc.Waves.Add((S_Wave)waves[i].Clone());
            }
            return sdvc;
        }
    }
}