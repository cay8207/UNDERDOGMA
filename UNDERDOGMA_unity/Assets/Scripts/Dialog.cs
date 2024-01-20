using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public List<string> sentences = new List<string>();
    public List<Sprite> sprites = new List<Sprite>();
    public List<Sprite> dialogueWindow = new List<Sprite>();

    public void Init()
    {
        sentences.Add("안녕하세요");
        sentences.Add("반갑습니다");
        sentences.Add("잘가요");
        sentences.Add("다음에 또 만나요");
        sentences.Add("안녕히가세요");
    }
}


