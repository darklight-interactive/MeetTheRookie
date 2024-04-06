using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Darklight.Bot.Tester
{
	public class TestTaskQueen : MonoBehaviour
	{
		static TaskBotQueen taskQueen;
		static TaskBotQueen asyncTaskQueen;

		public void Awake()
		{
			taskQueen = new GameObject("TaskQueen").AddComponent<TaskBotQueen>();
			asyncTaskQueen = new GameObject("AsyncTaskQueen").AddComponent<TaskBotQueen>();
		}

		public void Start()
		{
			//taskQueen.Initialize("TaskQueen");
			//asyncTaskQueen.Initialize("AsyncTaskQueen");
		}

		//[EasyButtons.Button]
		public void EnqueueAndExecuteTests()
		{
			TaskBot taskBot = new TaskBot(taskQueen, "TestTaskBot", async () =>
			{
				for (int i = 0; i < 100; i++)
				{
					Debug.Log("TestTaskBot: " + i);
					await Task.Delay(1000);
				}
				Debug.Log("TestTaskBot completed.");
			});

			taskQueen.Enqueue(taskBot);
			asyncTaskQueen.Enqueue(taskBot);
			taskQueen.ExecuteAllTasks();
			asyncTaskQueen.ExecuteAllTasks();
		}
	}
}