using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;

public class LogoTask : BaseTask
{
    public LogoTask(BaseState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();
        UIManager.CreatePanel<LogoPanel>(WindowTypeEnum.Screen);
    }

    public override void Exit()
    {
        base.Exit();

    }
}
