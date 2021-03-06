using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Transform _transform;

    private static UIManager m_instance;
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<UIManager>();
            return m_instance;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////

    private void Start()
    {
        _transform = transform;
    }

    public void ChangeText(string name, string text)
    {
        _transform.Find(name).GetComponent<Text>().text = text;
    }

    public void ChangeImage(string name, Sprite sprite)
    {
        _transform.Find(name).GetComponent<Image>().sprite = sprite;
    }

    public void SetActive(string name, bool state)
    {
        _transform.Find(name).gameObject.SetActive(state);
    }
}