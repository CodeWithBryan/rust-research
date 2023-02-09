using System;
using UnityEngine;

// Token: 0x02000621 RID: 1569
[Serializable]
public sealed class ByteQuadtree
{
	// Token: 0x06002CFB RID: 11515 RVA: 0x0010DDC0 File Offset: 0x0010BFC0
	public void UpdateValues(byte[] baseValues)
	{
		this.size = Mathf.RoundToInt(Mathf.Sqrt((float)baseValues.Length));
		this.levels = Mathf.RoundToInt(Mathf.Log((float)this.size, 2f)) + 1;
		this.values = new ByteMap[this.levels];
		this.values[0] = new ByteMap(this.size, baseValues, 1);
		for (int i = 1; i < this.levels; i++)
		{
			ByteMap byteMap = this.values[i - 1];
			ByteMap byteMap2 = this.values[i] = this.CreateLevel(i);
			for (int j = 0; j < byteMap2.Size; j++)
			{
				for (int k = 0; k < byteMap2.Size; k++)
				{
					byteMap2[k, j] = byteMap[2 * k, 2 * j] + byteMap[2 * k + 1, 2 * j] + byteMap[2 * k, 2 * j + 1] + byteMap[2 * k + 1, 2 * j + 1];
				}
			}
		}
	}

	// Token: 0x17000372 RID: 882
	// (get) Token: 0x06002CFC RID: 11516 RVA: 0x0010DED1 File Offset: 0x0010C0D1
	public int Size
	{
		get
		{
			return this.size;
		}
	}

	// Token: 0x17000373 RID: 883
	// (get) Token: 0x06002CFD RID: 11517 RVA: 0x0010DED9 File Offset: 0x0010C0D9
	public ByteQuadtree.Element Root
	{
		get
		{
			return new ByteQuadtree.Element(this, 0, 0, this.levels - 1);
		}
	}

	// Token: 0x06002CFE RID: 11518 RVA: 0x0010DEEC File Offset: 0x0010C0EC
	private ByteMap CreateLevel(int level)
	{
		int num = 1 << this.levels - level - 1;
		int bytes = 1 + (level + 3) / 4;
		return new ByteMap(num, bytes);
	}

	// Token: 0x040024F1 RID: 9457
	[SerializeField]
	private int size;

	// Token: 0x040024F2 RID: 9458
	[SerializeField]
	private int levels;

	// Token: 0x040024F3 RID: 9459
	[SerializeField]
	private ByteMap[] values;

	// Token: 0x02000D3E RID: 3390
	public struct Element
	{
		// Token: 0x06004E4D RID: 20045 RVA: 0x0019AB15 File Offset: 0x00198D15
		public Element(ByteQuadtree source, int x, int y, int level)
		{
			this.source = source;
			this.x = x;
			this.y = y;
			this.level = level;
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x06004E4E RID: 20046 RVA: 0x0019AB34 File Offset: 0x00198D34
		public bool IsLeaf
		{
			get
			{
				return this.level == 0;
			}
		}

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x06004E4F RID: 20047 RVA: 0x0019AB3F File Offset: 0x00198D3F
		public bool IsRoot
		{
			get
			{
				return this.level == this.source.levels - 1;
			}
		}

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x06004E50 RID: 20048 RVA: 0x0019AB56 File Offset: 0x00198D56
		public int ByteMap
		{
			get
			{
				return this.level;
			}
		}

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x06004E51 RID: 20049 RVA: 0x0019AB5E File Offset: 0x00198D5E
		public uint Value
		{
			get
			{
				return this.source.values[this.level][this.x, this.y];
			}
		}

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x06004E52 RID: 20050 RVA: 0x0019AB83 File Offset: 0x00198D83
		public Vector2 Coords
		{
			get
			{
				return new Vector2((float)this.x, (float)this.y);
			}
		}

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x06004E53 RID: 20051 RVA: 0x0019AB98 File Offset: 0x00198D98
		public int Depth
		{
			get
			{
				return this.source.levels - this.level - 1;
			}
		}

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x06004E54 RID: 20052 RVA: 0x0019ABAE File Offset: 0x00198DAE
		public ByteQuadtree.Element Parent
		{
			get
			{
				if (this.IsRoot)
				{
					throw new Exception("Element is the root and therefore has no parent.");
				}
				return new ByteQuadtree.Element(this.source, this.x / 2, this.y / 2, this.level + 1);
			}
		}

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x06004E55 RID: 20053 RVA: 0x0019ABE6 File Offset: 0x00198DE6
		public ByteQuadtree.Element Child1
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2, this.level - 1);
			}
		}

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x06004E56 RID: 20054 RVA: 0x0019AC1E File Offset: 0x00198E1E
		public ByteQuadtree.Element Child2
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2, this.level - 1);
			}
		}

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x06004E57 RID: 20055 RVA: 0x0019AC58 File Offset: 0x00198E58
		public ByteQuadtree.Element Child3
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2 + 1, this.level - 1);
			}
		}

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x06004E58 RID: 20056 RVA: 0x0019AC92 File Offset: 0x00198E92
		public ByteQuadtree.Element Child4
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2 + 1, this.level - 1);
			}
		}

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x06004E59 RID: 20057 RVA: 0x0019ACD0 File Offset: 0x00198ED0
		public ByteQuadtree.Element MaxChild
		{
			get
			{
				ByteQuadtree.Element child = this.Child1;
				ByteQuadtree.Element child2 = this.Child2;
				ByteQuadtree.Element child3 = this.Child3;
				ByteQuadtree.Element child4 = this.Child4;
				uint value = child.Value;
				uint value2 = child2.Value;
				uint value3 = child3.Value;
				uint value4 = child4.Value;
				if (value >= value2 && value >= value3 && value >= value4)
				{
					return child;
				}
				if (value2 >= value3 && value2 >= value4)
				{
					return child2;
				}
				if (value3 >= value4)
				{
					return child3;
				}
				return child4;
			}
		}

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x06004E5A RID: 20058 RVA: 0x0019AD48 File Offset: 0x00198F48
		public ByteQuadtree.Element RandChild
		{
			get
			{
				ByteQuadtree.Element child = this.Child1;
				ByteQuadtree.Element child2 = this.Child2;
				ByteQuadtree.Element child3 = this.Child3;
				ByteQuadtree.Element child4 = this.Child4;
				uint value = child.Value;
				uint value2 = child2.Value;
				uint value3 = child3.Value;
				uint value4 = child4.Value;
				float num = value + value2 + value3 + value4;
				float value5 = UnityEngine.Random.value;
				if (value / num >= value5)
				{
					return child;
				}
				if ((value + value2) / num >= value5)
				{
					return child2;
				}
				if ((value + value2 + value3) / num >= value5)
				{
					return child3;
				}
				return child4;
			}
		}

		// Token: 0x0400459F RID: 17823
		private ByteQuadtree source;

		// Token: 0x040045A0 RID: 17824
		private int x;

		// Token: 0x040045A1 RID: 17825
		private int y;

		// Token: 0x040045A2 RID: 17826
		private int level;
	}
}
