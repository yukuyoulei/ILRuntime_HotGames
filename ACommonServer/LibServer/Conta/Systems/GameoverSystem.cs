using System;
using System.Collections.Generic;
using System.Text;

public class GameoverSystem : SystemBase
{
	public GameoverSystem(ContaBase conta) : base(conta)
	{
	}

	public override void Tick(double fDeltaSec)
	{
		if (!conta.enabled) return;
		if (!conta.gameover) return;
		conta.destroy();
		if (string.IsNullOrEmpty(conta.failed_reason))
			conta.GameOverSuccess();
		else
			conta.GameOverFailed(conta.failed_reason);
	}
}
