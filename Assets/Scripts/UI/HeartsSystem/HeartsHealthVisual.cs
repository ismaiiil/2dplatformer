using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsHealthVisual : MonoBehaviour
{
    #pragma warning disable CS0649
    [SerializeField] private Sprite heartSprite_full;
    [SerializeField] private Sprite heartSprite_half; 

    private List<HeartImage> heartImageList;

    private void Awake()
    {
        heartImageList = new List<HeartImage>();
    }

    private void Start()
    {

    }
    private HeartImage CreateHeartImage(Vector2 anchoredPostion) {
        GameObject heartGameObject = new GameObject("Heart", typeof(Image));
        heartGameObject.transform.SetParent(transform,false);
        heartGameObject.transform.localPosition = Vector3.zero;
        heartGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPostion;
        heartGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100,100);
        var heartImage = heartGameObject.GetComponent<Image>();
        heartImage.sprite = heartSprite_full;

        var imageObj = new HeartImage(this,heartImage);
        heartImageList.Add(imageObj);
        return imageObj;
    }

    public void drawLives(int lifeAmount) {
        DestroyAllLives();
        for (int i = 0; i <= lifeAmount-1; i++)
        {
            CreateHeartImage(new Vector2(100*i, 0));
        }
    }

    public void RemoveLife(float amount) {

        if (amount == 0.5f)
        {
            if (heartImageList.Count - 1 < 0)
            {
                return;
            }
            var currentHeart = heartImageList[heartImageList.Count - 1];
            if (currentHeart.currentValue == 0.5f)
            {
                heartImageList.Remove(currentHeart);
                Destroy(currentHeart.heartImage);
            }
            else
            {
                currentHeart.SetHeartFragments(1);
                currentHeart.currentValue = 0.5f;
            }
        }
        else if (amount == 1.0f)
        {
            if (heartImageList.Count - 1 < 0)
            {
                return;
            }
            var currentHeart = heartImageList[heartImageList.Count - 1];
            if (currentHeart.currentValue == 0.5f)
            {
                heartImageList.Remove(currentHeart);
                Destroy(currentHeart.heartImage);
                if (heartImageList.Count - 1 < 0)
                {
                    return;
                }
                var nexttHeart = heartImageList[heartImageList.Count - 1];
                nexttHeart.SetHeartFragments(1);
                nexttHeart.currentValue = 0.5f;
            }
            else
            {
                heartImageList.Remove(currentHeart);
                Destroy(currentHeart.heartImage);
            }

        }

    }

    public void Addlife(float amount)
    {
        if (amount == 0.5f)
        {
            if (heartImageList.Count - 1 < 0)
            {
                return;
            }
            var currentHeart = heartImageList[heartImageList.Count - 1];
            if (currentHeart.currentValue == 0.5f)
            {
                currentHeart.SetHeartFragments(0);
                currentHeart.currentValue = 1.0f;
            }
            else
            {
                RectTransform rectTransform = currentHeart.heartImage.GetComponent<RectTransform>();
                var newHeart = CreateHeartImage(new Vector2(rectTransform.anchoredPosition.x + 100, rectTransform.anchoredPosition.y));
                newHeart.SetHeartFragments(1);
                newHeart.currentValue = 0.5f;
            }
        }
        else if (amount == 1.0f)
        {
            if (heartImageList.Count - 1 < 0)
            {
                return;
            }
            var currentHeart = heartImageList[heartImageList.Count - 1];
            if (currentHeart.currentValue == 0.5f)
            {
                currentHeart.SetHeartFragments(0);
                currentHeart.currentValue = 1.0f;

                RectTransform rectTransform = currentHeart.heartImage.GetComponent<RectTransform>();
                var newHeart = CreateHeartImage(new Vector2(rectTransform.anchoredPosition.x + 100, rectTransform.anchoredPosition.y));
                newHeart.SetHeartFragments(1);
                newHeart.currentValue = 0.5f;
            }
            else
            {
                RectTransform rectTransform = currentHeart.heartImage.GetComponent<RectTransform>();
                var newHeart = CreateHeartImage(new Vector2(rectTransform.anchoredPosition.x + 100, rectTransform.anchoredPosition.y));
            }

        }
        
    }

    private void DestroyAllLives() {
        foreach (var heart in heartImageList)
        {
            Destroy(heart.heartImage);
        }
        heartImageList = new List<HeartImage>();
    }

    public bool CheckIsDead() {
        if (heartImageList.Count == 0)
        {
            return true;
        }
        return false;
    }

    private class HeartImage {

        public Image heartImage;
        private HeartsHealthVisual heartsHealthVisual;
        public float currentValue;

        public HeartImage(HeartsHealthVisual heartsHealthVisual, Image heartImage) {
            this.heartsHealthVisual = heartsHealthVisual;
            this.heartImage = heartImage;            
            currentValue = 1;
        }

        public void SetHeartFragments(int Fragments) {
            switch (Fragments)
            {
                case 0: heartImage.sprite = heartsHealthVisual.heartSprite_full; break;
                case 1: heartImage.sprite = heartsHealthVisual.heartSprite_half; break;
            }
        }

    }
}
