using System;
using UnityEngine;

// Token: 0x020001DA RID: 474
public class AIThinkManager : BaseMonoBehaviour, IServerComponent
{
	// Token: 0x060018D4 RID: 6356 RVA: 0x000B5698 File Offset: 0x000B3898
	public static void ProcessQueue(AIThinkManager.QueueType queueType)
	{
		if (queueType != AIThinkManager.QueueType.Human)
		{
		}
		if (queueType == AIThinkManager.QueueType.Human)
		{
			AIThinkManager.DoRemoval(AIThinkManager._removalQueue, AIThinkManager._processQueue);
			AIInformationZone.BudgetedTick();
		}
		else if (queueType == AIThinkManager.QueueType.Pets)
		{
			AIThinkManager.DoRemoval(AIThinkManager._petRemovalQueue, AIThinkManager._petProcessQueue);
		}
		else
		{
			AIThinkManager.DoRemoval(AIThinkManager._animalremovalQueue, AIThinkManager._animalProcessQueue);
		}
		if (queueType == AIThinkManager.QueueType.Human)
		{
			AIThinkManager.DoProcessing(AIThinkManager._processQueue, AIThinkManager.framebudgetms / 1000f, ref AIThinkManager.lastIndex);
			return;
		}
		if (queueType == AIThinkManager.QueueType.Pets)
		{
			AIThinkManager.DoProcessing(AIThinkManager._petProcessQueue, AIThinkManager.petframebudgetms / 1000f, ref AIThinkManager.lastPetIndex);
			return;
		}
		AIThinkManager.DoProcessing(AIThinkManager._animalProcessQueue, AIThinkManager.animalframebudgetms / 1000f, ref AIThinkManager.lastAnimalIndex);
	}

	// Token: 0x060018D5 RID: 6357 RVA: 0x000B5744 File Offset: 0x000B3944
	private static void DoRemoval(ListHashSet<IThinker> removal, ListHashSet<IThinker> process)
	{
		if (removal.Count > 0)
		{
			foreach (IThinker val in removal)
			{
				process.Remove(val);
			}
			removal.Clear();
		}
	}

	// Token: 0x060018D6 RID: 6358 RVA: 0x000B57A4 File Offset: 0x000B39A4
	private static void DoProcessing(ListHashSet<IThinker> process, float budgetSeconds, ref int last)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (last < process.Count && Time.realtimeSinceStartup < realtimeSinceStartup + budgetSeconds)
		{
			IThinker thinker = process[last];
			if (thinker != null)
			{
				try
				{
					thinker.TryThink();
				}
				catch (Exception message)
				{
					Debug.LogWarning(message);
				}
			}
			last++;
		}
		if (last >= process.Count)
		{
			last = 0;
		}
	}

	// Token: 0x060018D7 RID: 6359 RVA: 0x000B580C File Offset: 0x000B3A0C
	public static void Add(IThinker toAdd)
	{
		AIThinkManager._processQueue.Add(toAdd);
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x000B5819 File Offset: 0x000B3A19
	public static void Remove(IThinker toRemove)
	{
		AIThinkManager._removalQueue.Add(toRemove);
	}

	// Token: 0x060018D9 RID: 6361 RVA: 0x000B5826 File Offset: 0x000B3A26
	public static void AddAnimal(IThinker toAdd)
	{
		AIThinkManager._animalProcessQueue.Add(toAdd);
	}

	// Token: 0x060018DA RID: 6362 RVA: 0x000B5833 File Offset: 0x000B3A33
	public static void RemoveAnimal(IThinker toRemove)
	{
		AIThinkManager._animalremovalQueue.Add(toRemove);
	}

	// Token: 0x060018DB RID: 6363 RVA: 0x000B5840 File Offset: 0x000B3A40
	public static void AddPet(IThinker toAdd)
	{
		AIThinkManager._petProcessQueue.Add(toAdd);
	}

	// Token: 0x060018DC RID: 6364 RVA: 0x000B584D File Offset: 0x000B3A4D
	public static void RemovePet(IThinker toRemove)
	{
		AIThinkManager._petRemovalQueue.Add(toRemove);
	}

	// Token: 0x040011C9 RID: 4553
	public static ListHashSet<IThinker> _processQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040011CA RID: 4554
	public static ListHashSet<IThinker> _removalQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040011CB RID: 4555
	public static ListHashSet<IThinker> _animalProcessQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040011CC RID: 4556
	public static ListHashSet<IThinker> _animalremovalQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040011CD RID: 4557
	public static ListHashSet<IThinker> _petProcessQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040011CE RID: 4558
	public static ListHashSet<IThinker> _petRemovalQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040011CF RID: 4559
	[ServerVar]
	[Help("How many miliseconds to budget for processing AI entities per server frame")]
	public static float framebudgetms = 2.5f;

	// Token: 0x040011D0 RID: 4560
	[ServerVar]
	[Help("How many miliseconds to budget for processing animal AI entities per server frame")]
	public static float animalframebudgetms = 2.5f;

	// Token: 0x040011D1 RID: 4561
	[ServerVar]
	[Help("How many miliseconds to budget for processing pet AI entities per server frame")]
	public static float petframebudgetms = 1f;

	// Token: 0x040011D2 RID: 4562
	private static int lastIndex = 0;

	// Token: 0x040011D3 RID: 4563
	private static int lastAnimalIndex = 0;

	// Token: 0x040011D4 RID: 4564
	private static int lastPetIndex;

	// Token: 0x02000BFD RID: 3069
	public enum QueueType
	{
		// Token: 0x04004055 RID: 16469
		Human,
		// Token: 0x04004056 RID: 16470
		Animal,
		// Token: 0x04004057 RID: 16471
		Pets
	}
}
