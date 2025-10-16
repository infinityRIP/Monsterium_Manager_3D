using System.Collections.Generic;
using events;
using UnityEngine;
using UnityEngine.UI;

namespace demo
{
    /// <summary>
    /// Clones a card on hover and shows it as a preview above the original.
    /// </summary>
    public class ClonePreviewManager : MonoBehaviour, CardPreviewManager
    {
        [SerializeField] private float verticalPosition = 0f;
        [SerializeField] private float previewScale = 1f;
        [SerializeField] private int previewSortingOrder = 100;

        // Prefer explicit ctor for older Unity/C# toolchains
        private readonly Dictionary<CardWrapper, Transform> previews =
            new Dictionary<CardWrapper, Transform>();

        public void OnCardHover(CardHover e)
        {
            if (e == null || e.card == null) return;
            OnCardPreviewStarted(e.card);
        }

        public void OnCardUnhover(CardUnhover e)
        {
            if (e == null || e.card == null) return;
            OnCardPreviewEnded(e.card);
        }

        public void OnCardPreviewStarted(CardWrapper card)
        {
            if (card == null) return;

            if (!previews.ContainsKey(card))
            {
                CreateCloneForCard(card);
            }

            if (!previews.TryGetValue(card, out var preview) || preview == null) return;

            var cardPos = card.transform.position;
            preview.gameObject.SetActive(true);
            preview.position = new Vector3(cardPos.x, verticalPosition, cardPos.z);
        }

        public void OnCardPreviewEnded(CardWrapper card)
        {
            if (card == null) return;
            if (!previews.TryGetValue(card, out var preview) || preview == null) return;

            preview.gameObject.SetActive(false);
        }

        private void CreateCloneForCard(CardWrapper card)
        {
            var cloneGO = Instantiate(card.gameObject, transform);
            cloneGO.name = $"{card.gameObject.name}_Preview";
            var t = cloneGO.transform;

            t.position = card.transform.position;
            t.localScale = Vector3.one * previewScale;
            t.rotation = Quaternion.identity;

            // Find a Canvas on this object or its children, then ensure sorting works.
            var canvas = cloneGO.GetComponent<Canvas>() ?? cloneGO.GetComponentInChildren<Canvas>(true);
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = previewSortingOrder;
            }

            StripCloneComponents(cloneGO);

            // Start hidden; we’ll show it in OnCardPreviewStarted.
            cloneGO.SetActive(false);

            // Remember: key is the ORIGINAL card; value is the clone transform.
            previews[card] = t;
        }

        private static void StripCloneComponents(GameObject clone)
        {
            // Remove gameplay/interactive bits on the clone so it’s visual-only.
            var wrapper = clone.GetComponent<CardWrapper>();
            if (wrapper) Destroy(wrapper);

            foreach (var r in clone.GetComponentsInChildren<GraphicRaycaster>(true))
                Destroy(r);

            foreach (var c in clone.GetComponentsInChildren<Collider>(true))
                Destroy(c);

            foreach (var c2 in clone.GetComponentsInChildren<Collider2D>(true))
                Destroy(c2);

            // Optional: freeze any custom behaviours on the clone
            // foreach (var mb in clone.GetComponentsInChildren<MonoBehaviour>(true))
            //     mb.enabled = false;
        }

        private void OnDestroy()
        {
            // Cleanup previews if this manager is destroyed.
            foreach (var kv in previews)
            {
                if (kv.Value != null)
                    Destroy(kv.Value.gameObject);
            }
            previews.Clear();
        }
    }
}
