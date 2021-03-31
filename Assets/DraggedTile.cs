using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggedTile : MonoBehaviour
{
    public BoardSlot hoveredSlot;
    public BoardSlot slotDraggedFrom;
    public BoardSlot currentSlotDraggedTo;
    public MatchObject draggedObject;

    public bool dragLeftRight = false;
    public void updateDraggedObject(MatchObject matchObjectDragged)
    {
        draggedObject.matchIcon = matchObjectDragged.matchIcon;
        draggedObject.tileMatchType = matchObjectDragged.tileMatchType;
    }
}
