using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MatrixCreator
{
	public partial class MatrixCreator : Form
	{	
		public MatrixCreator()
		{
			InitializeComponent();
			matrix_table.AutoGenerateColumns = true;
			RezizeMatrix();
		}

		private void RezizeMatrix()
		{
			while (matrix_table.ColumnCount != matrix_size.Value)
			{
				if (matrix_size.Value > matrix_table.ColumnCount)
				{
					matrix_table.Columns.AddRange(new DataGridViewTextBoxColumn());
					matrix_table.Rows.Add();
					result_matrix.Rows.Add();
				}
				else
				{
					matrix_table.Rows.RemoveAt(matrix_table.Rows.Count - 1);
					matrix_table.Columns.RemoveAt(matrix_table.Columns.Count - 1);
					result_matrix.Rows.RemoveAt(result_matrix.Rows.Count - 1);
				}
			}
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			RezizeMatrix();
		}

		private void ValidateIsNumber(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			if (e.Control is not TextBox tb) return;

			tb.KeyPress += (object? snd, KeyPressEventArgs kpe) =>
			{
				if (!char.IsControl(kpe.KeyChar) && !char.IsDigit(kpe.KeyChar))
					kpe.Handled = true;
			};
		}

		private void SaveToJson(DataGridView matrixData, DataGridView resultData)
		{
			float[,] matrix = new float[matrixData.ColumnCount, matrixData.RowCount];
			float[] results = new float[resultData.RowCount];

			var toJson = new ArrayList();
			toJson.Add(matrix);
			toJson.Add(results);

			for (int rows = 0; rows < matrixData.Rows.Count; rows++)
			{
				for (int col = 0; col < matrixData.Rows[rows].Cells.Count; col++)
				{
					var value = matrixData.Rows[rows].Cells[col].Value ?? "0";
					matrix[rows, col] = float.Parse(value.ToString()!);
				}
			}

			for (int rows = 0; rows < resultData.Rows.Count; rows++)
			{
				var value = resultData.Rows[rows].Cells[0].Value ?? "0";
				results[rows] = float.Parse(value.ToString()!);
			}

			string json = JsonConvert.SerializeObject(toJson, Formatting.Indented) ?? "";
			var sfd = new SaveFileDialog();

			sfd.Filter = "JSON Files (*.json)|*.json|txt files (*.txt)|*.txt|All files (*.*)|*.*";

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				File.WriteAllText(sfd.FileName, json);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			SaveToJson(matrix_table, result_matrix);
		}
	}
}
