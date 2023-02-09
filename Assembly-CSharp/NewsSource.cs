using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Facepunch.Models;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200084E RID: 2126
public class NewsSource : MonoBehaviour
{
	// Token: 0x060034D3 RID: 13523 RVA: 0x0013EB98 File Offset: 0x0013CD98
	public void Awake()
	{
		GA.DesignEvent("news:view");
	}

	// Token: 0x060034D4 RID: 13524 RVA: 0x0013EBA4 File Offset: 0x0013CDA4
	public void OnEnable()
	{
		if (SteamNewsSource.Stories == null || SteamNewsSource.Stories.Length == 0)
		{
			return;
		}
		this.SetStory(SteamNewsSource.Stories[0]);
	}

	// Token: 0x060034D5 RID: 13525 RVA: 0x0013EBC8 File Offset: 0x0013CDC8
	public void SetStory(SteamNewsSource.Story story)
	{
		NewsSource.<>c__DisplayClass12_0 CS$<>8__locals1 = new NewsSource.<>c__DisplayClass12_0();
		CS$<>8__locals1.story = story;
		PlayerPrefs.SetInt("lastNewsDate", CS$<>8__locals1.story.date);
		this.container.DestroyAllChildren(false);
		this.title.text = CS$<>8__locals1.story.name;
		this.authorName.text = "by " + CS$<>8__locals1.story.author;
		string str = ((long)(Epoch.Current - CS$<>8__locals1.story.date)).FormatSecondsLong();
		this.date.text = "Posted " + str + " ago";
		this.button.onClick.RemoveAllListeners();
		this.button.onClick.AddListener(delegate()
		{
			Facepunch.Models.Manifest.NewsInfo.BlogInfo blogInfo2 = base.<SetStory>g__GetBlogPost|1();
			string text2 = ((blogInfo2 != null) ? blogInfo2.Url : null) ?? CS$<>8__locals1.story.url;
			Debug.Log("Opening URL: " + text2);
			UnityEngine.Application.OpenURL(text2);
		});
		Facepunch.Models.Manifest.NewsInfo.BlogInfo blogInfo = CS$<>8__locals1.<SetStory>g__GetBlogPost|1();
		string text = (blogInfo != null) ? blogInfo.HeaderImage : null;
		NewsSource.ParagraphBuilder paragraphBuilder = NewsSource.ParagraphBuilder.New();
		this.ParseBbcode(ref paragraphBuilder, CS$<>8__locals1.story.text, ref text, 0);
		this.AppendParagraph(ref paragraphBuilder);
		if (text != null)
		{
			this.coverImage.Load(text);
		}
		RustText[] componentsInChildren = this.container.GetComponentsInChildren<RustText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DoAutoSize();
		}
	}

	// Token: 0x060034D6 RID: 13526 RVA: 0x0013ED0C File Offset: 0x0013CF0C
	private void ParseBbcode(ref NewsSource.ParagraphBuilder currentParagraph, string bbcode, ref string firstImage, int depth = 0)
	{
		foreach (object obj in NewsSource.BbcodeParse.Matches(bbcode))
		{
			Match match = (Match)obj;
			string value = match.Groups[1].Value;
			string value2 = match.Groups[2].Value;
			string value3 = match.Groups[3].Value;
			string value4 = match.Groups[4].Value;
			currentParagraph.Append(value);
			string text = value2.ToLowerInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2369466585U)
			{
				if (num <= 706259085U)
				{
					if (num != 217798785U)
					{
						if (num != 632598351U)
						{
							if (num == 706259085U)
							{
								if (text == "noparse")
								{
									currentParagraph.Append(value4);
								}
							}
						}
						else if (text == "strike")
						{
							currentParagraph.Append("<s>");
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</s>");
						}
					}
					else if (text == "list")
					{
						currentParagraph.AppendLine();
						foreach (string text2 in NewsSource.GetBulletPoints(value4))
						{
							if (!string.IsNullOrWhiteSpace(text2))
							{
								currentParagraph.Append("\t• ");
								currentParagraph.Append(text2.Trim());
								currentParagraph.AppendLine();
							}
						}
					}
				}
				else if (num <= 1624406948U)
				{
					if (num != 848251934U)
					{
						if (num == 1624406948U)
						{
							if (text == "previewyoutube")
							{
								if (depth == 0)
								{
									string[] array2 = value3.Split(new char[]
									{
										';'
									});
									this.AppendYouTube(ref currentParagraph, array2[0]);
								}
							}
						}
					}
					else if (text == "url")
					{
						if (value4.Contains("[img]", StringComparison.InvariantCultureIgnoreCase))
						{
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth);
						}
						else
						{
							int count = currentParagraph.Links.Count;
							currentParagraph.Links.Add(value3);
							currentParagraph.Append(string.Format("<link={0}><u>", count));
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</u></link>");
						}
					}
				}
				else if (num != 2229740804U)
				{
					if (num == 2369466585U)
					{
						if (text == "h4")
						{
							currentParagraph.Append("<size=150%>");
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</size>");
						}
					}
				}
				else if (text == "img")
				{
					if (depth == 0)
					{
						string text3 = value4.Trim();
						if (firstImage == null)
						{
							firstImage = text3;
						}
						this.AppendImage(ref currentParagraph, text3);
					}
				}
			}
			else if (num <= 2419799442U)
			{
				if (num != 2386244204U)
				{
					if (num != 2403021823U)
					{
						if (num != 2419799442U)
						{
							continue;
						}
						if (!(text == "h1"))
						{
							continue;
						}
					}
					else if (!(text == "h2"))
					{
						continue;
					}
					currentParagraph.Append("<size=200%>");
					this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
					currentParagraph.Append("</size>");
				}
				else if (text == "h3")
				{
					currentParagraph.Append("<size=175%>");
					this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
					currentParagraph.Append("</size>");
				}
			}
			else if (num <= 3876335077U)
			{
				if (num != 2791659946U)
				{
					if (num == 3876335077U)
					{
						if (text == "b")
						{
							currentParagraph.Append("<b>");
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</b>");
						}
					}
				}
				else if (text == "olist")
				{
					currentParagraph.AppendLine();
					string[] bulletPoints = NewsSource.GetBulletPoints(value4);
					int num2 = 1;
					foreach (string text4 in bulletPoints)
					{
						if (!string.IsNullOrWhiteSpace(text4))
						{
							currentParagraph.Append(string.Format("\t{0} ", num2++));
							currentParagraph.Append(text4.Trim());
							currentParagraph.AppendLine();
						}
					}
				}
			}
			else if (num != 3960223172U)
			{
				if (num == 4027333648U)
				{
					if (text == "u")
					{
						currentParagraph.Append("<u>");
						this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
						currentParagraph.Append("</u>");
					}
				}
			}
			else if (text == "i")
			{
				currentParagraph.Append("<i>");
				this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</i>");
			}
		}
	}

	// Token: 0x060034D7 RID: 13527 RVA: 0x0013F298 File Offset: 0x0013D498
	private static string[] GetBulletPoints(string listContent)
	{
		return ((listContent != null) ? listContent.Split(NewsSource.BulletSeparators, StringSplitOptions.RemoveEmptyEntries) : null) ?? Array.Empty<string>();
	}

	// Token: 0x060034D8 RID: 13528 RVA: 0x0013F2B8 File Offset: 0x0013D4B8
	private void AppendParagraph(ref NewsSource.ParagraphBuilder currentParagraph)
	{
		if (currentParagraph.StringBuilder.Length > 0)
		{
			string text = currentParagraph.StringBuilder.ToString();
			RustText rustText = UnityEngine.Object.Instantiate<RustText>(this.paragraphTemplate, this.container);
			rustText.SetActive(true);
			rustText.SetText(text);
			NewsParagraph newsParagraph;
			if (rustText.TryGetComponent<NewsParagraph>(out newsParagraph))
			{
				newsParagraph.Links = currentParagraph.Links;
			}
		}
		currentParagraph = NewsSource.ParagraphBuilder.New();
	}

	// Token: 0x060034D9 RID: 13529 RVA: 0x0013F31E File Offset: 0x0013D51E
	private void AppendImage(ref NewsSource.ParagraphBuilder currentParagraph, string url)
	{
		this.AppendParagraph(ref currentParagraph);
		HttpImage httpImage = UnityEngine.Object.Instantiate<HttpImage>(this.imageTemplate, this.container);
		httpImage.SetActive(true);
		httpImage.Load(url);
	}

	// Token: 0x060034DA RID: 13530 RVA: 0x0013F348 File Offset: 0x0013D548
	private void AppendYouTube(ref NewsSource.ParagraphBuilder currentParagraph, string videoId)
	{
		this.AppendParagraph(ref currentParagraph);
		HttpImage httpImage = UnityEngine.Object.Instantiate<HttpImage>(this.youtubeTemplate, this.container);
		httpImage.SetActive(true);
		httpImage.Load("https://img.youtube.com/vi/" + videoId + "/maxresdefault.jpg");
		RustButton component = httpImage.GetComponent<RustButton>();
		if (component != null)
		{
			string videoUrl = "https://www.youtube.com/watch?v=" + videoId;
			component.OnReleased.AddListener(delegate()
			{
				Debug.Log("Opening URL: " + videoUrl);
				UnityEngine.Application.OpenURL(videoUrl);
			});
		}
	}

	// Token: 0x04002F53 RID: 12115
	private static readonly Regex BbcodeParse = new Regex("([^\\[]*)(?:\\[(\\w+)(?:=([^\\]]+))?\\](.*?)\\[\\/\\2\\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

	// Token: 0x04002F54 RID: 12116
	public RustText title;

	// Token: 0x04002F55 RID: 12117
	public RustText date;

	// Token: 0x04002F56 RID: 12118
	public RustText authorName;

	// Token: 0x04002F57 RID: 12119
	public HttpImage coverImage;

	// Token: 0x04002F58 RID: 12120
	public RectTransform container;

	// Token: 0x04002F59 RID: 12121
	public Button button;

	// Token: 0x04002F5A RID: 12122
	public RustText paragraphTemplate;

	// Token: 0x04002F5B RID: 12123
	public HttpImage imageTemplate;

	// Token: 0x04002F5C RID: 12124
	public HttpImage youtubeTemplate;

	// Token: 0x04002F5D RID: 12125
	private static readonly string[] BulletSeparators = new string[]
	{
		"[*]"
	};

	// Token: 0x02000E35 RID: 3637
	private struct ParagraphBuilder
	{
		// Token: 0x06005011 RID: 20497 RVA: 0x001A0CBC File Offset: 0x0019EEBC
		public static NewsSource.ParagraphBuilder New()
		{
			return new NewsSource.ParagraphBuilder
			{
				StringBuilder = new StringBuilder(),
				Links = new List<string>()
			};
		}

		// Token: 0x06005012 RID: 20498 RVA: 0x001A0CEA File Offset: 0x0019EEEA
		public void AppendLine()
		{
			this.StringBuilder.AppendLine();
		}

		// Token: 0x06005013 RID: 20499 RVA: 0x001A0CF8 File Offset: 0x0019EEF8
		public void Append(string text)
		{
			this.StringBuilder.Append(text);
		}

		// Token: 0x040049A1 RID: 18849
		public StringBuilder StringBuilder;

		// Token: 0x040049A2 RID: 18850
		public List<string> Links;
	}
}
