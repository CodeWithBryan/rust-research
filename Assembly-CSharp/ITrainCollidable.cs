using System;

// Token: 0x02000487 RID: 1159
public interface ITrainCollidable
{
	// Token: 0x060025BB RID: 9659
	bool CustomCollision(TrainCar train, TriggerTrainCollisions trainTrigger);

	// Token: 0x060025BC RID: 9660
	bool EqualNetID(BaseNetworkable other);
}
