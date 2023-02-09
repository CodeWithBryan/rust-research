using System;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class Buttons
{
	// Token: 0x02000C50 RID: 3152
	public class ConButton : ConsoleSystem.IConsoleButton
	{
		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x06004C81 RID: 19585 RVA: 0x00195C73 File Offset: 0x00193E73
		// (set) Token: 0x06004C82 RID: 19586 RVA: 0x00195C7B File Offset: 0x00193E7B
		public bool IsDown { get; set; }

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x06004C83 RID: 19587 RVA: 0x00195C84 File Offset: 0x00193E84
		public bool JustPressed
		{
			get
			{
				return this.IsDown && this.frame == Time.frameCount;
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06004C84 RID: 19588 RVA: 0x00195C9D File Offset: 0x00193E9D
		public bool JustReleased
		{
			get
			{
				return !this.IsDown && this.frame == Time.frameCount;
			}
		}

		// Token: 0x06004C85 RID: 19589 RVA: 0x000059DD File Offset: 0x00003BDD
		public void Call(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06004C86 RID: 19590 RVA: 0x00195CB6 File Offset: 0x00193EB6
		// (set) Token: 0x06004C87 RID: 19591 RVA: 0x00195CBE File Offset: 0x00193EBE
		public bool IsPressed
		{
			get
			{
				return this.IsDown;
			}
			set
			{
				if (value == this.IsDown)
				{
					return;
				}
				this.IsDown = value;
				this.frame = Time.frameCount;
			}
		}

		// Token: 0x040041BF RID: 16831
		private int frame;
	}
}
