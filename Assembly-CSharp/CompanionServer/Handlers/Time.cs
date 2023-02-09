using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	// Token: 0x020009CE RID: 2510
	public class Time : BaseHandler<AppEmpty>
	{
		// Token: 0x06003B5A RID: 15194 RVA: 0x0015B544 File Offset: 0x00159744
		public override void Execute()
		{
			TOD_Sky instance = TOD_Sky.Instance;
			TOD_Time time = instance.Components.Time;
			AppTime appTime = Pool.Get<AppTime>();
			appTime.dayLengthMinutes = time.DayLengthInMinutes;
			appTime.timeScale = (time.ProgressTime ? Time.timeScale : 0f);
			appTime.sunrise = instance.SunriseTime;
			appTime.sunset = instance.SunsetTime;
			appTime.time = instance.Cycle.Hour;
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.time = appTime;
			base.Send(appResponse);
		}
	}
}
