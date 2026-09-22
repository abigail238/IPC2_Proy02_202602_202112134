using System;

namespace IPC2_Proy02_202602_202112134.Models
{

    public class Libro

{
	//codigo unico que identifica el libro
	public int ISBN { get; set; }

	public string Titulo { get; set; }

	public string Autor { get; set; }

	public string Categoria { get; set; }


	//contructor
	public Libro(int isbn, string titulo, string autor, string categoria)
	{
		ISBN = isbn;
		Titulo = titulo;
		Autor = autor;
		Categoria = categoria;
	}
  
   }
}
