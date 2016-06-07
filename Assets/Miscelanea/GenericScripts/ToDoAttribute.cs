using UnityEngine;
using System.Collections;

public class ToDoAttribute : System.Attribute
{
    public string Message;
    public enum ToDoType { Remove, Document, BugFix, Other };
    public ToDoType Task;
    public ToDoAttribute() { }
    public ToDoAttribute(string Message) { this.Message = Message; }
    public ToDoAttribute(string Message, ToDoType type) { this.Message = Message; Task = type; }
    public ToDoAttribute(ToDoType type) { Task = type; }
}
