using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cube : MonoBehaviour
{
    [SerializeField] private Text letter = null;
    [SerializeField] private Renderer rend = null;
    [SerializeField] private GameObject defaultMesh = null;
    [SerializeField] private GameObject keyMesh = null;
    public WordStack stack;
    public int Index = 0;
    private CubeType type = CubeType.GOOD;

    public CubeType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
            if (type == CubeType.KEY)
            {
                defaultMesh.SetActive(false);
                keyMesh.SetActive(true);
                letter.enabled = false;
            }
        }
    }

    private void Awake()
    {
        defaultMesh.SetActive(true);
        keyMesh.SetActive(false);
        letter.enabled = true;
    }

    public Renderer Renderer
    {
        get
        {
            return rend;
        }
    }

    public void SetLetter(char letter)
    {
        this.letter.text = letter.ToString();
    }

    public enum CubeType { WRONG, GOOD, KEY }

}
