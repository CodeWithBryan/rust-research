using System;

// Token: 0x02000446 RID: 1094
public class BaseSaddle : BaseMountable
{
	// Token: 0x060023DC RID: 9180 RVA: 0x000E28B6 File Offset: 0x000E0AB6
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (player != this._mounted)
		{
			return;
		}
		if (this.animal)
		{
			this.animal.RiderInput(inputState, player);
		}
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x000E28E1 File Offset: 0x000E0AE1
	public void SetAnimal(BaseRidableAnimal newAnimal)
	{
		this.animal = newAnimal;
	}

	// Token: 0x04001C71 RID: 7281
	public BaseRidableAnimal animal;
}
