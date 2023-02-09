using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Network;
using UnityEngine;

// Token: 0x020002E6 RID: 742
[ConsoleSystem.Factory("global")]
public class DiagnosticsConSys : ConsoleSystem
{
	// Token: 0x06001D3B RID: 7483 RVA: 0x000C77D0 File Offset: 0x000C59D0
	private static void DumpAnimators(string targetFolder)
	{
		Animator[] array = UnityEngine.Object.FindObjectsOfType<Animator>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All animators");
		stringBuilder.AppendLine();
		foreach (Animator animator in array)
		{
			stringBuilder.AppendFormat("{1}\t{0}", animator.transform.GetRecursiveName(""), animator.enabled);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.List.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All animators - grouped by object name");
		stringBuilder2.AppendLine();
		foreach (IGrouping<string, Animator> source in from x in array
		group x by x.transform.GetRecursiveName("") into x
		orderby x.Count<Animator>() descending
		select x)
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", source.First<Animator>().transform.GetRecursiveName(""), source.Count<Animator>());
			stringBuilder2.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.txt", stringBuilder2.ToString());
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.AppendLine("All animators - grouped by enabled/disabled");
		stringBuilder3.AppendLine();
		foreach (IGrouping<string, Animator> source2 in from x in array
		group x by x.transform.GetRecursiveName(x.enabled ? "" : " (DISABLED)") into x
		orderby x.Count<Animator>() descending
		select x)
		{
			stringBuilder3.AppendFormat("{1:N0}\t{0}", source2.First<Animator>().transform.GetRecursiveName(source2.First<Animator>().enabled ? "" : " (DISABLED)"), source2.Count<Animator>());
			stringBuilder3.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.Enabled.txt", stringBuilder3.ToString());
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x000C7A40 File Offset: 0x000C5C40
	private static void DumpEntities(string targetFolder)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All entities");
		stringBuilder.AppendLine();
		foreach (BaseNetworkable baseNetworkable in BaseNetworkable.serverEntities)
		{
			stringBuilder.AppendFormat("{1}\t{0}", baseNetworkable.PrefabName, (baseNetworkable.net != null) ? baseNetworkable.net.ID : 0U);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.List.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All entities");
		stringBuilder2.AppendLine();
		foreach (IGrouping<uint, BaseNetworkable> source in from x in BaseNetworkable.serverEntities
		group x by x.prefabID into x
		orderby x.Count<BaseNetworkable>() descending
		select x)
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", source.First<BaseNetworkable>().PrefabName, source.Count<BaseNetworkable>());
			stringBuilder2.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.Counts.txt", stringBuilder2.ToString());
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.AppendLine("Saved entities");
		stringBuilder3.AppendLine();
		foreach (IGrouping<uint, BaseEntity> source2 in from x in BaseEntity.saveList
		group x by x.prefabID into x
		orderby x.Count<BaseEntity>() descending
		select x)
		{
			stringBuilder3.AppendFormat("{1:N0}\t{0}", source2.First<BaseEntity>().PrefabName, source2.Count<BaseEntity>());
			stringBuilder3.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.Savelist.Counts.txt", stringBuilder3.ToString());
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x000C7CA8 File Offset: 0x000C5EA8
	private static void DumpLODGroups(string targetFolder)
	{
		DiagnosticsConSys.DumpLODGroupTotals(targetFolder);
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x000C7CB0 File Offset: 0x000C5EB0
	private static void DumpLODGroupTotals(string targetFolder)
	{
		IEnumerable<LODGroup> source = UnityEngine.Object.FindObjectsOfType<LODGroup>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("LODGroups");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, LODGroup> grouping in from x in source
		group x by x.transform.GetRecursiveName("") into x
		orderby x.Count<LODGroup>() descending
		select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", grouping.Key, grouping.Count<LODGroup>());
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "LODGroups.Objects.txt", stringBuilder.ToString());
	}

	// Token: 0x06001D3F RID: 7487 RVA: 0x000C7D90 File Offset: 0x000C5F90
	private static void DumpNetwork(string targetFolder)
	{
		if (Net.sv.IsConnected())
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Server Network Statistics");
			stringBuilder.AppendLine();
			stringBuilder.Append(Net.sv.GetDebug(null).Replace("\n", "\r\n"));
			stringBuilder.AppendLine();
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				stringBuilder.AppendLine("Name: " + basePlayer.displayName);
				stringBuilder.AppendLine("SteamID: " + basePlayer.userID);
				stringBuilder.Append((basePlayer.net == null) ? "INVALID - NET IS NULL" : Net.sv.GetDebug(basePlayer.net.connection).Replace("\n", "\r\n"));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
			}
			DiagnosticsConSys.WriteTextToFile(targetFolder + "Network.Server.txt", stringBuilder.ToString());
		}
	}

	// Token: 0x06001D40 RID: 7488 RVA: 0x000C7ED0 File Offset: 0x000C60D0
	private static void DumpObjects(string targetFolder)
	{
		UnityEngine.Object[] source = UnityEngine.Object.FindObjectsOfType<UnityEngine.Object>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active UnityEngine.Object, ordered by count");
		stringBuilder.AppendLine();
		foreach (IGrouping<Type, UnityEngine.Object> source2 in from x in source
		group x by x.GetType() into x
		orderby x.Count<UnityEngine.Object>() descending
		select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", source2.First<UnityEngine.Object>().GetType().Name, source2.Count<UnityEngine.Object>());
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Object.Count.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All active UnityEngine.ScriptableObject, ordered by count");
		stringBuilder2.AppendLine();
		foreach (IGrouping<Type, UnityEngine.Object> source3 in from x in source
		where x is ScriptableObject
		group x by x.GetType() into x
		orderby x.Count<UnityEngine.Object>() descending
		select x)
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", source3.First<UnityEngine.Object>().GetType().Name, source3.Count<UnityEngine.Object>());
			stringBuilder2.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.ScriptableObject.Count.txt", stringBuilder2.ToString());
	}

	// Token: 0x06001D41 RID: 7489 RVA: 0x000C80C0 File Offset: 0x000C62C0
	private static void DumpPhysics(string targetFolder)
	{
		DiagnosticsConSys.DumpTotals(targetFolder);
		DiagnosticsConSys.DumpColliders(targetFolder);
		DiagnosticsConSys.DumpRigidBodies(targetFolder);
	}

	// Token: 0x06001D42 RID: 7490 RVA: 0x000C80D4 File Offset: 0x000C62D4
	private static void DumpTotals(string targetFolder)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Information");
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<Collider>().Count<Collider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Active Colliders:\t{0:N0}", (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
		where x.enabled
		select x).Count<Collider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total RigidBodys:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<Rigidbody>().Count<Rigidbody>());
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Mesh Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<MeshCollider>().Count<MeshCollider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Box Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<BoxCollider>().Count<BoxCollider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Sphere Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<SphereCollider>().Count<SphereCollider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Capsule Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<CapsuleCollider>().Count<CapsuleCollider>());
		stringBuilder.AppendLine();
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.txt", stringBuilder.ToString());
	}

	// Token: 0x06001D43 RID: 7491 RVA: 0x000C822C File Offset: 0x000C642C
	private static void DumpColliders(string targetFolder)
	{
		IEnumerable<Collider> source = UnityEngine.Object.FindObjectsOfType<Collider>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Colliders");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Collider> grouping in from x in source
		group x by x.transform.GetRecursiveName("") into x
		orderby x.Count<Collider>() descending
		select x)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			string format = "{1:N0}\t{0} ({2:N0} triggers) ({3:N0} enabled)";
			object[] array = new object[4];
			array[0] = grouping.Key;
			array[1] = grouping.Count<Collider>();
			array[2] = grouping.Count((Collider x) => x.isTrigger);
			array[3] = grouping.Count((Collider x) => x.enabled);
			stringBuilder2.AppendFormat(format, array);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.Colliders.Objects.txt", stringBuilder.ToString());
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x000C8378 File Offset: 0x000C6578
	private static void DumpRigidBodies(string targetFolder)
	{
		IEnumerable<Rigidbody> source = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("RigidBody");
		stringBuilder.AppendLine();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("RigidBody");
		stringBuilder2.AppendLine();
		foreach (IGrouping<string, Rigidbody> grouping in from x in source
		group x by x.transform.GetRecursiveName("") into x
		orderby x.Count<Rigidbody>() descending
		select x)
		{
			StringBuilder stringBuilder3 = stringBuilder;
			string format = "{1:N0}\t{0} ({2:N0} awake) ({3:N0} kinematic) ({4:N0} non-discrete)";
			object[] array = new object[5];
			array[0] = grouping.Key;
			array[1] = grouping.Count<Rigidbody>();
			array[2] = grouping.Count((Rigidbody x) => !x.IsSleeping());
			array[3] = grouping.Count((Rigidbody x) => x.isKinematic);
			array[4] = grouping.Count((Rigidbody x) => x.collisionDetectionMode > CollisionDetectionMode.Discrete);
			stringBuilder3.AppendFormat(format, array);
			stringBuilder.AppendLine();
			foreach (Rigidbody rigidbody in grouping)
			{
				stringBuilder2.AppendFormat("{0} -{1}{2}{3}", new object[]
				{
					grouping.Key,
					rigidbody.isKinematic ? " KIN" : "",
					rigidbody.IsSleeping() ? " SLEEP" : "",
					rigidbody.useGravity ? " GRAVITY" : ""
				});
				stringBuilder2.AppendLine();
				stringBuilder2.AppendFormat("Mass: {0}\tVelocity: {1}\tsleepThreshold: {2}", rigidbody.mass, rigidbody.velocity, rigidbody.sleepThreshold);
				stringBuilder2.AppendLine();
				stringBuilder2.AppendLine();
			}
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.RigidBody.Objects.txt", stringBuilder.ToString());
		DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.RigidBody.All.txt", stringBuilder2.ToString());
	}

	// Token: 0x06001D45 RID: 7493 RVA: 0x000C861C File Offset: 0x000C681C
	private static void DumpGameObjects(string targetFolder)
	{
		Transform[] rootObjects = TransformUtil.GetRootObjects();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active game objects");
		stringBuilder.AppendLine();
		foreach (Transform tx in rootObjects)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(stringBuilder, tx, 0, false);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Hierarchy.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active game objects including components");
		stringBuilder.AppendLine();
		foreach (Transform tx2 in rootObjects)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(stringBuilder, tx2, 0, true);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Hierarchy.Components.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects excluding children");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Transform> source in from x in rootObjects
		group x by x.name into x
		orderby x.Count<Transform>() descending
		select x)
		{
			Transform transform = source.First<Transform>();
			stringBuilder.AppendFormat("{1:N0}\t{0}", transform.name, source.Count<Transform>());
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Count.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects including children");
		stringBuilder.AppendLine();
		foreach (KeyValuePair<Transform, int> keyValuePair in from x in rootObjects
		group x by x.name into x
		select new KeyValuePair<Transform, int>(x.First<Transform>(), x.Sum((Transform y) => y.GetAllChildren().Count)) into x
		orderby x.Value descending
		select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", keyValuePair.Key.name, keyValuePair.Value);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Count.Children.txt", stringBuilder.ToString());
	}

	// Token: 0x06001D46 RID: 7494 RVA: 0x000C88B8 File Offset: 0x000C6AB8
	private static void DumpGameObjectRecursive(StringBuilder str, Transform tx, int indent, bool includeComponents = false)
	{
		if (tx == null)
		{
			return;
		}
		for (int i = 0; i < indent; i++)
		{
			str.Append(" ");
		}
		str.AppendFormat("{0} {1:N0}", tx.name, tx.GetComponents<Component>().Length - 1);
		str.AppendLine();
		if (includeComponents)
		{
			foreach (Component component in tx.GetComponents<Component>())
			{
				if (!(component is Transform))
				{
					for (int k = 0; k < indent + 1; k++)
					{
						str.Append(" ");
					}
					str.AppendFormat("[c] {0}", (component == null) ? "NULL" : component.GetType().ToString());
					str.AppendLine();
				}
			}
		}
		for (int l = 0; l < tx.childCount; l++)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(str, tx.GetChild(l), indent + 2, includeComponents);
		}
	}

	// Token: 0x06001D47 RID: 7495 RVA: 0x000C89A8 File Offset: 0x000C6BA8
	[ServerVar]
	[ClientVar]
	public static void dump(ConsoleSystem.Arg args)
	{
		if (Directory.Exists("diagnostics"))
		{
			Directory.CreateDirectory("diagnostics");
		}
		int num = 1;
		while (Directory.Exists("diagnostics/" + num))
		{
			num++;
		}
		Directory.CreateDirectory("diagnostics/" + num);
		string targetFolder = "diagnostics/" + num + "/";
		DiagnosticsConSys.DumpLODGroups(targetFolder);
		DiagnosticsConSys.DumpSystemInformation(targetFolder);
		DiagnosticsConSys.DumpGameObjects(targetFolder);
		DiagnosticsConSys.DumpObjects(targetFolder);
		DiagnosticsConSys.DumpEntities(targetFolder);
		DiagnosticsConSys.DumpNetwork(targetFolder);
		DiagnosticsConSys.DumpPhysics(targetFolder);
		DiagnosticsConSys.DumpAnimators(targetFolder);
	}

	// Token: 0x06001D48 RID: 7496 RVA: 0x000C8A45 File Offset: 0x000C6C45
	private static void DumpSystemInformation(string targetFolder)
	{
		DiagnosticsConSys.WriteTextToFile(targetFolder + "System.Info.txt", SystemInfoGeneralText.currentInfo);
	}

	// Token: 0x06001D49 RID: 7497 RVA: 0x000C8A5C File Offset: 0x000C6C5C
	private static void WriteTextToFile(string file, string text)
	{
		File.WriteAllText(file, text);
	}
}
