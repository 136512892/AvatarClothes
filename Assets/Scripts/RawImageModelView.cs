using UnityEngine;
using UnityEngine.EventSystems;

namespace SK.Framework 
{
    /// <summary>
    /// 使用RawImage显示3D模型
    /// </summary>
    public class RawImageModelView : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
    {
        //模型
        [SerializeField] private GameObject model;
        //水平方向
        [SerializeField] private bool horizontal = true;
        //垂直方向
        [SerializeField] private bool vertical = true;
        //灵敏度
        [SerializeField] private float sensitivity = 100f;
        //鼠标按下
        private bool isPressed;

        private void Update()
        {
            if (model == null) return;
            if (Input.GetMouseButton(0) && isPressed)
            {
                float x = horizontal ? Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity : 0f;
                float y = vertical ? Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity : 0f;
                model.transform.Rotate(y, -x, 0f, Space.World);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            isPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
        }
    }
}