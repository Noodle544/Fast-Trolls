﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Team
{
    public string TeamName;
    public int TeamIndex;
    public DateTime LastSpawn;
    public Color TeamColor;
    public List<Bonom> Members = new List<Bonom>();
    public Engine Engine;
    public Flag Flag;
    public Squad[] Squads;

    public float DamageDealt = 0;
    public float DamageRecieved = 0;
    public float DamageHealed = 0;
    public int KillCount = 0;

    public Team(Engine engine, int teamIndex, Color teamColor, string teamName = null)
    {
        Engine = engine;
        TeamName = teamName == null ? $"Team {teamIndex}" : teamName;
        TeamIndex = teamIndex;
        TeamColor = teamColor;
        LastSpawn = DateTime.Now;
        Flag = Engine.GenerateTeamFlag(this);
        Squads = new Squad[engine.PreSets.Length];
        float sum = 0;
        float init = 1f / Squads.Length;
        Debug.Log(init);
        for (int i = 0; i < Squads.Length; i++)
        {
            Squads[i] = new Squad(i, i < Squads.Length - 1 ? init : 1 - sum);
            sum += init;
        } 
    }

    private BonomStats NeededStats()
    {
        for (int i = 0; i < Squads.Length; i++)
        {
            if (Squads[i].Ratio == 0)
                continue;

            float currentRatio = (float)Squads[i].Count / Members.Count;

            if (Members.Count == 0 ||
                Squads[i].Count == 0 ||
                currentRatio < Squads[i].Ratio)
            {
                return Engine.PreSets[i];
            }
                
        }

        return Engine.PreSets[0]; // fail-safe
    }

    public void AddBonom(Bonom newBonom, bool random = false)
    {
        newBonom.Init(Engine, this, random ? Engine.RandomBonomStats() : NeededStats());
        Members.Add(newBonom);
        Squad targetSquad = this[Engine.GetTypeIndex(newBonom.Stats.Type)];
        targetSquad.Count++;

        Engine.UIManager.CountUpdate(this);
    }

    public void RemoveBonom(Bonom targetBonom)
    {
        if (Members.Remove(targetBonom))
            this[Engine.GetTypeIndex(targetBonom.Stats.Type)].Count--;
    }

    public int SquadIndex(int type)
    {
        for (int i = 0; i < Squads.Length; i++)
            if (Squads[i].TypeIndex == type)
                return i;
        return -1;
    }

    public Squad this[int type]
    {
        get
        {
            for (int i = 0; i < Squads.Length; i++)
                if (Squads[i].TypeIndex == type)
                    return Squads[i];
            return null;
        }

        set
        {
            for (int i = 0; i < Squads.Length; i++)
                if (Squads[i].TypeIndex == type)
                    Squads[i] = value;
        }
    }
}
