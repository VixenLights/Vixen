using System;
using System.Collections;
using System.Collections.Generic;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	class OutputDeviceCollectionExecutionMediator<T> : IOutputDeviceManager<T> 
		where T : class, IOutputDevice {
		private IOutputDeviceCollection<T> _deviceCollection;
		private IOutputDeviceExecution<T> _deviceExecution;

		public OutputDeviceCollectionExecutionMediator(IOutputDeviceCollection<T> deviceCollection, IOutputDeviceExecution<T> deviceExecution) {
			_deviceCollection = deviceCollection;
			_deviceExecution = deviceExecution;
		}

		public IOutputDevice GetDevice(Guid id) {
			return _deviceCollection.Get(id);
		}

		public IEnumerable<IOutputDevice> Devices {
			get { return this; }
		}

		public void Start(T outputDevice) {
			_deviceExecution.Start(outputDevice);
		}

		public void Stop(T outputDevice) {
			_deviceExecution.Stop(outputDevice);
		}

		public void Pause(T outputDevice) {
			_deviceExecution.Pause(outputDevice);
		}

		public void Resume(T outputDevice) {
			_deviceExecution.Resume(outputDevice);
		}

		public void StartAll() {
			_deviceExecution.StartAll();
		}

		public void StopAll() {
			_deviceExecution.StopAll();
		}

		public void PauseAll() {
			_deviceExecution.PauseAll();
		}

		public void ResumeAll() {
			_deviceExecution.ResumeAll();
		}

		public void StartAll(IEnumerable<T> outputDevices) {
			_deviceExecution.StartAll(outputDevices);
		}

		public void StopAll(IEnumerable<T> outputDevices) {
			_deviceExecution.StopAll(outputDevices);
		}

		public void PauseAll(IEnumerable<T> outputDevices) {
			_deviceExecution.PauseAll(outputDevices);
		}

		public void ResumeAll(IEnumerable<T> outputDevices) {
			_deviceExecution.ResumeAll(outputDevices);
		}

		public void Add(T outputDevice) {
			_deviceCollection.Add(outputDevice);

			// Make sure the device is running/not running like all the others.
			switch(_deviceExecution.ExecutionState) {
				case ExecutionState.Started:
					Start(outputDevice);
					break;
				case ExecutionState.Stopped:
					Stop(outputDevice);
					break;
				case ExecutionState.Paused:
					Start(outputDevice);
					Pause(outputDevice);
					break;
			}
		}

		public void AddRange(IEnumerable<T> outputDevices) {
			foreach(T outputDevice in outputDevices) {
				Add(outputDevice);
			}
		}

		public bool Remove(T outputDevice) {
			if(_deviceCollection.Remove(outputDevice)) {
				Stop(outputDevice);
				return true;
			}
			return false;
		}

		public T Get(Guid id) {
			return _deviceCollection.Get(id);
		}

		public IEnumerable<T> GetAll() {
			return _deviceCollection.GetAll();
		}

		public ExecutionState ExecutionState {
			get { return _deviceExecution.ExecutionState; }
		}

		public IEnumerator<T> GetEnumerator() {
			return _deviceCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
