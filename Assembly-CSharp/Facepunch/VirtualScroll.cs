using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Facepunch
{
	// Token: 0x02000AAB RID: 2731
	public class VirtualScroll : MonoBehaviour
	{
		// Token: 0x0600415E RID: 16734 RVA: 0x0017FE71 File Offset: 0x0017E071
		public void Awake()
		{
			this.ScrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnScrollChanged));
			if (this.DataSourceObject != null)
			{
				this.SetDataSource(this.DataSourceObject.GetComponent<VirtualScroll.IDataSource>(), false);
			}
		}

		// Token: 0x0600415F RID: 16735 RVA: 0x0017FEAF File Offset: 0x0017E0AF
		public void OnDestroy()
		{
			this.ScrollRect.onValueChanged.RemoveListener(new UnityAction<Vector2>(this.OnScrollChanged));
		}

		// Token: 0x06004160 RID: 16736 RVA: 0x0017FECD File Offset: 0x0017E0CD
		private void OnScrollChanged(Vector2 pos)
		{
			this.Rebuild();
		}

		// Token: 0x06004161 RID: 16737 RVA: 0x0017FED5 File Offset: 0x0017E0D5
		public void SetDataSource(VirtualScroll.IDataSource source, bool forceRebuild = false)
		{
			if (this.dataSource == source && !forceRebuild)
			{
				return;
			}
			this.dataSource = source;
			this.FullRebuild();
		}

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x06004162 RID: 16738 RVA: 0x0017FEF1 File Offset: 0x0017E0F1
		private int BlockHeight
		{
			get
			{
				return this.ItemHeight + this.ItemSpacing;
			}
		}

		// Token: 0x06004163 RID: 16739 RVA: 0x0017FF00 File Offset: 0x0017E100
		public void FullRebuild()
		{
			foreach (int key in this.ActivePool.Keys.ToArray<int>())
			{
				this.Recycle(key);
			}
			this.Rebuild();
		}

		// Token: 0x06004164 RID: 16740 RVA: 0x0017FF40 File Offset: 0x0017E140
		public void DataChanged()
		{
			foreach (KeyValuePair<int, GameObject> keyValuePair in this.ActivePool)
			{
				this.dataSource.SetItemData(keyValuePair.Key, keyValuePair.Value);
			}
			this.Rebuild();
		}

		// Token: 0x06004165 RID: 16741 RVA: 0x0017FFAC File Offset: 0x0017E1AC
		public void Rebuild()
		{
			if (this.dataSource == null)
			{
				return;
			}
			int itemCount = this.dataSource.GetItemCount();
			RectTransform rectTransform = (this.OverrideContentRoot != null) ? this.OverrideContentRoot : (this.ScrollRect.viewport.GetChild(0) as RectTransform);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)(this.BlockHeight * itemCount - this.ItemSpacing + this.Padding.top + this.Padding.bottom));
			int num = Mathf.Max(2, Mathf.CeilToInt(this.ScrollRect.viewport.rect.height / (float)this.BlockHeight));
			int num2 = Mathf.FloorToInt((rectTransform.anchoredPosition.y - (float)this.Padding.top) / (float)this.BlockHeight);
			int num3 = num2 + num;
			this.RecycleOutOfRange(num2, (float)num3);
			for (int i = num2; i <= num3; i++)
			{
				if (i >= 0 && i < itemCount)
				{
					this.BuildItem(i);
				}
			}
		}

		// Token: 0x06004166 RID: 16742 RVA: 0x001800AC File Offset: 0x0017E2AC
		private void RecycleOutOfRange(int startVisible, float endVisible)
		{
			foreach (int key in (from x in this.ActivePool.Keys
			where x < startVisible || (float)x > endVisible
			select x).ToArray<int>())
			{
				this.Recycle(key);
			}
		}

		// Token: 0x06004167 RID: 16743 RVA: 0x0018012C File Offset: 0x0017E32C
		private void Recycle(int key)
		{
			GameObject gameObject = this.ActivePool[key];
			gameObject.SetActive(false);
			this.ActivePool.Remove(key);
			this.InactivePool.Push(gameObject);
		}

		// Token: 0x06004168 RID: 16744 RVA: 0x00180168 File Offset: 0x0017E368
		private void BuildItem(int i)
		{
			if (i < 0)
			{
				return;
			}
			if (this.ActivePool.ContainsKey(i))
			{
				return;
			}
			GameObject item = this.GetItem();
			item.SetActive(true);
			this.dataSource.SetItemData(i, item);
			RectTransform rectTransform = item.transform as RectTransform;
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(1f, 1f);
			rectTransform.pivot = new Vector2(0.5f, 1f);
			rectTransform.offsetMin = new Vector2(0f, 0f);
			rectTransform.offsetMax = new Vector2(0f, (float)this.ItemHeight);
			rectTransform.sizeDelta = new Vector2((float)((this.Padding.left + this.Padding.right) * -1), (float)this.ItemHeight);
			rectTransform.anchoredPosition = new Vector2((float)(this.Padding.left - this.Padding.right) * 0.5f, (float)(-1 * (i * this.BlockHeight + this.Padding.top)));
			this.ActivePool[i] = item;
		}

		// Token: 0x06004169 RID: 16745 RVA: 0x00180294 File Offset: 0x0017E494
		private GameObject GetItem()
		{
			if (this.InactivePool.Count == 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.SourceObject);
				gameObject.transform.SetParent((this.OverrideContentRoot != null) ? this.OverrideContentRoot : this.ScrollRect.viewport.GetChild(0), false);
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(false);
				this.InactivePool.Push(gameObject);
			}
			return this.InactivePool.Pop();
		}

		// Token: 0x04003A50 RID: 14928
		public int ItemHeight = 40;

		// Token: 0x04003A51 RID: 14929
		public int ItemSpacing = 10;

		// Token: 0x04003A52 RID: 14930
		public RectOffset Padding;

		// Token: 0x04003A53 RID: 14931
		[Tooltip("Optional, we'll try to GetComponent IDataSource from this object on awake")]
		public GameObject DataSourceObject;

		// Token: 0x04003A54 RID: 14932
		public GameObject SourceObject;

		// Token: 0x04003A55 RID: 14933
		public ScrollRect ScrollRect;

		// Token: 0x04003A56 RID: 14934
		public RectTransform OverrideContentRoot;

		// Token: 0x04003A57 RID: 14935
		private VirtualScroll.IDataSource dataSource;

		// Token: 0x04003A58 RID: 14936
		private Dictionary<int, GameObject> ActivePool = new Dictionary<int, GameObject>();

		// Token: 0x04003A59 RID: 14937
		private Stack<GameObject> InactivePool = new Stack<GameObject>();

		// Token: 0x02000F0D RID: 3853
		public interface IDataSource
		{
			// Token: 0x060051E0 RID: 20960
			int GetItemCount();

			// Token: 0x060051E1 RID: 20961
			void SetItemData(int i, GameObject obj);
		}
	}
}
