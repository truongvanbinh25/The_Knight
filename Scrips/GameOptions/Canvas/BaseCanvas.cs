using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseCanvas : MonoBehaviour
{
    protected virtual bool CheckOutCanvas(RectTransform rectTransform)
    {
        Vector2 mousePosition = Input.mousePosition;

        if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition)) // Kiểm tra xem chuột có nằm ngoài vùng canvas không
        {
            return true;
        }
        return false;
    }
}
