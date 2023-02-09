using System;
using System.Collections;
using System.IO;
using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x0200084B RID: 2123
public class MenuBackgroundVideo : SingletonComponent<MenuBackgroundVideo>
{
	// Token: 0x060034C7 RID: 13511 RVA: 0x0013E94F File Offset: 0x0013CB4F
	protected override void Awake()
	{
		base.Awake();
		this.LoadVideoList();
		this.NextVideo();
		base.GetComponent<VideoPlayer>().errorReceived += this.OnVideoError;
	}

	// Token: 0x060034C8 RID: 13512 RVA: 0x0013E97A File Offset: 0x0013CB7A
	private void OnVideoError(VideoPlayer source, string message)
	{
		this.errored = true;
	}

	// Token: 0x060034C9 RID: 13513 RVA: 0x0013E984 File Offset: 0x0013CB84
	public void LoadVideoList()
	{
		this.videos = (from x in Directory.EnumerateFiles(UnityEngine.Application.streamingAssetsPath + "/MenuVideo/")
		where x.EndsWith(".mp4") || x.EndsWith(".webm")
		orderby Guid.NewGuid()
		select x).ToArray<string>();
	}

	// Token: 0x060034CA RID: 13514 RVA: 0x0013E9F8 File Offset: 0x0013CBF8
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			this.LoadVideoList();
		}
		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			this.NextVideo();
		}
	}

	// Token: 0x060034CB RID: 13515 RVA: 0x0013EA20 File Offset: 0x0013CC20
	private void NextVideo()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		string[] array = this.videos;
		int num = this.index;
		this.index = num + 1;
		string text = array[num % this.videos.Length];
		this.errored = false;
		if (Global.LaunchCountThisVersion <= 3)
		{
			string text2 = (from x in this.videos
			where x.EndsWith("whatsnew.mp4")
			select x).FirstOrDefault<string>();
			if (!string.IsNullOrEmpty(text2))
			{
				text = text2;
			}
		}
		Debug.Log("Playing Video " + text);
		VideoPlayer component = base.GetComponent<VideoPlayer>();
		component.url = text;
		component.Play();
	}

	// Token: 0x060034CC RID: 13516 RVA: 0x0013EAC2 File Offset: 0x0013CCC2
	internal IEnumerator ReadyVideo()
	{
		if (this.errored)
		{
			yield break;
		}
		VideoPlayer player = base.GetComponent<VideoPlayer>();
		while (!player.isPrepared)
		{
			if (this.errored)
			{
				yield break;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04002F4E RID: 12110
	private string[] videos;

	// Token: 0x04002F4F RID: 12111
	private int index;

	// Token: 0x04002F50 RID: 12112
	private bool errored;
}
