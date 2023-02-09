using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000870 RID: 2160
public class UIParticle : BaseMonoBehaviour
{
	// Token: 0x06003543 RID: 13635 RVA: 0x0014029C File Offset: 0x0013E49C
	public static void Add(UIParticle particleSource, RectTransform spawnPosition, RectTransform particleCanvas)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(particleSource.gameObject);
		gameObject.transform.SetParent(spawnPosition, false);
		gameObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(0f, spawnPosition.rect.width) - spawnPosition.rect.width * spawnPosition.pivot.x, UnityEngine.Random.Range(0f, spawnPosition.rect.height) - spawnPosition.rect.height * spawnPosition.pivot.y, 0f);
		gameObject.transform.SetParent(particleCanvas, true);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06003544 RID: 13636 RVA: 0x00140368 File Offset: 0x0013E568
	private void Start()
	{
		base.transform.localScale *= UnityEngine.Random.Range(this.InitialScale.x, this.InitialScale.y);
		this.velocity.x = UnityEngine.Random.Range(this.InitialX.x, this.InitialX.y);
		this.velocity.y = UnityEngine.Random.Range(this.InitialY.x, this.InitialY.y);
		this.gravity = UnityEngine.Random.Range(this.Gravity.x, this.Gravity.y);
		this.scaleVelocity = UnityEngine.Random.Range(this.ScaleVelocity.x, this.ScaleVelocity.y);
		Image component = base.GetComponent<Image>();
		if (component)
		{
			component.color = this.InitialColor.Evaluate(UnityEngine.Random.Range(0f, 1f));
		}
		this.lifetime = UnityEngine.Random.Range(this.InitialDelay.x, this.InitialDelay.y) * -1f;
		if (this.lifetime < 0f)
		{
			base.GetComponent<CanvasGroup>().alpha = 0f;
		}
		base.Invoke(new Action(this.Die), UnityEngine.Random.Range(this.LifeTime.x, this.LifeTime.y) + this.lifetime * -1f);
	}

	// Token: 0x06003545 RID: 13637 RVA: 0x001404E4 File Offset: 0x0013E6E4
	private void Update()
	{
		if (this.lifetime < 0f)
		{
			this.lifetime += Time.deltaTime;
			if (this.lifetime < 0f)
			{
				return;
			}
			base.GetComponent<CanvasGroup>().alpha = 1f;
		}
		else
		{
			this.lifetime += Time.deltaTime;
		}
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.localScale;
		this.velocity.y = this.velocity.y - this.gravity * Time.deltaTime;
		position.x += this.velocity.x * Time.deltaTime;
		position.y += this.velocity.y * Time.deltaTime;
		vector += Vector3.one * this.scaleVelocity * Time.deltaTime;
		if (vector.x <= 0f || vector.y <= 0f)
		{
			this.Die();
			return;
		}
		base.transform.position = position;
		base.transform.localScale = vector;
	}

	// Token: 0x06003546 RID: 13638 RVA: 0x00140607 File Offset: 0x0013E807
	private void Die()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x04002FDA RID: 12250
	public Vector2 LifeTime;

	// Token: 0x04002FDB RID: 12251
	public Vector2 Gravity = new Vector2(1000f, 1000f);

	// Token: 0x04002FDC RID: 12252
	public Vector2 InitialX;

	// Token: 0x04002FDD RID: 12253
	public Vector2 InitialY;

	// Token: 0x04002FDE RID: 12254
	public Vector2 InitialScale = Vector2.one;

	// Token: 0x04002FDF RID: 12255
	public Vector2 InitialDelay;

	// Token: 0x04002FE0 RID: 12256
	public Vector2 ScaleVelocity;

	// Token: 0x04002FE1 RID: 12257
	public Gradient InitialColor;

	// Token: 0x04002FE2 RID: 12258
	private float lifetime;

	// Token: 0x04002FE3 RID: 12259
	private float gravity;

	// Token: 0x04002FE4 RID: 12260
	private Vector2 velocity;

	// Token: 0x04002FE5 RID: 12261
	private float scaleVelocity;
}
