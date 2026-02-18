using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class AutoGridLayout : GridLayoutGroup
{
	public bool autoWidth = true;
	public bool autoHeight = false;

	[SerializeField] int columns = 8;
	
	public bool controlAspectRatio = false;
	public float aspectRatio = 1f;

	public override void SetLayoutHorizontal()
	{
		UpdateCellSize();
		base.SetLayoutHorizontal();
	}

	public override void SetLayoutVertical()
	{
		UpdateCellSize();
		base.SetLayoutVertical();
	}

	void UpdateCellSize()
	{
		if (!autoWidth && !autoHeight) return;

		if (autoWidth && !autoHeight)
		{
			if (columns <= 0) return;

			var rect = rectTransform.rect.width;
			float totalPadding = padding.left + padding.right;
			float totalSpacing = spacing.x * (columns - 1);

			float w = (rect - totalPadding - totalSpacing) / columns;

			if (controlAspectRatio)
			{
				if (aspectRatio <= 0f) return;

				float h = w / aspectRatio;
				cellSize = new Vector2(w, h);
			}
			else
			{
				cellSize = new Vector2(w, cellSize.y);
			}

			return;
		}

		if (!autoWidth && autoHeight)
		{
			int rows = GetRowsForAutoHeight();
			if (rows <= 0) return;

			var rect = rectTransform.rect.height;
			float totalPadding = padding.top + padding.bottom;
			float totalSpacing = spacing.y * (rows - 1);

			float h = (rect - totalPadding - totalSpacing) / rows;

			if (controlAspectRatio)
			{
				if (aspectRatio <= 0f) return;

				float w = h * aspectRatio;
				cellSize = new Vector2(w, h);
			}
			else
			{
				cellSize = new Vector2(cellSize.x, h);
			}

			return;
		}

		if (columns <= 0) return;

		{
			var rect = rectTransform.rect.width;
			float totalPadding = padding.left + padding.right;
			float totalSpacing = spacing.x * (columns - 1);

			float w = (rect - totalPadding - totalSpacing) / columns;
			cellSize = new Vector2(w, cellSize.y);
		}

		{
			int rows = GetRowsForAutoHeight();
			if (rows <= 0) return;

			var rect = rectTransform.rect.height;
			float totalPadding = padding.top + padding.bottom;
			float totalSpacing = spacing.y * (rows - 1);

			float h = (rect - totalPadding - totalSpacing) / rows;
			cellSize = new Vector2(cellSize.x, h);
		}
	}
	
	public void SetColumns(int columns)
	{
		this.columns = columns;
		constraintCount = columns;
		UpdateCellSize();
	}

	int GetRowsForAutoHeight()
	{
		if (constraint == Constraint.FixedRowCount)
		{
			return constraintCount;
		}

		int columnsForRows;
		if (constraint == Constraint.FixedColumnCount)
		{
			columnsForRows = constraintCount;
		}
		else
		{
			columnsForRows = columns;
		}

		if (columnsForRows <= 0) return 0;
		if (rectChildren.Count == 0) return 0;

		return Mathf.CeilToInt(rectChildren.Count / (float)columnsForRows);
	}
}
