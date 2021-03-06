using System;

namespace CodeEditor.Text.UI
{
	/// <summary>
	/// Maintains the zero based coordinates of the input caret.
	/// </summary>
	public interface ICaret
	{
		int Row { get; }
		int Column { get; }
		void SetPosition(int row, int column);
		void MoveLeft();
		void MoveRight();
		void MoveUp(int rows);
		void MoveDown(int rows);
		void MoveToRowStart();
		void MoveToRowEnd();
		void MoveToStart();
		void MoveToEnd();
		event Action Moved;
	}
}
