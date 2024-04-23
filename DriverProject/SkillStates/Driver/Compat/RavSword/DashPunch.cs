﻿namespace RobDriver.SkillStates.Driver.Compat
{
    public class DashPunch : DashGrab
    {
        protected override bool forcePunch => true;
        protected override string startAnimString => "DashPunchStart";
        protected override string dashAnimString => "DashPunch";
    }
}