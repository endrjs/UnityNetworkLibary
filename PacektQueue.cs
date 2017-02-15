using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PacektQueue : MonoBehaviour {
	// Saved packet informations
	struct PacketInfo{
		public int offset;
		public int size;
	};

	// Buffer for datas
	private MemoryStream m_streamBuffer;

	// Pacekt informations lists
	private List<PacketInfo>m_offsetList;

	// Memory offset
	private int m_offset = 0;

	// Lock object
	private System.Object lockObj = new System.Object(); // Object


	// Constructor
	public PacektQueue(){
		m_streamBuffer = new MemoryStream();
		m_offsetList = new List<PacketInfo>();
	}

	// Adding Queue
	public int Enqueue(byte[] data, int size){
		PacketInfo info = new PacketInfo ();
	// Packet Informations
		info.offset = m_offset;
		info.size = size;

		lock (lockObj) {
			// informations of saved packets
			m_offsetList.Add(info);
			// Packet datas
			m_streamBuffer.Position = m_offset;
			m_streamBuffer.Write (data,0,size);
			m_streamBuffer.Flush ();
			m_offset += size;
		}
		return size;
	}

	// Dequeue packets
	public int Dequeue(ref byte[] buffer, int size){

		if (m_offsetList.Count <= 0) {
			return  -1;
		 }

		int recvSize = 0;
		lock(lockObj){
			PacketInfo info = m_offsetList[0];
			// Get the packet from the buffer
			int dataSize = Math.Min(size, info.size);
			m_streamBuffer.Position = info.offset;
			recvSize = m_streamBuffer.Read(buffer, 0, dataSize);

			// Remove the first data from the Queue
			if(recvSize > 0){
				m_offsetList.RemoveAt(0);
			}

			// Arragne the streams to save memory
			if(m_offsetList.Count == 0){
				//Clear ();
				m_offset = 0;
			}
		}
		return recvSize;
	}

	// Arranging Queue
	public void Clear(){
		byte[] buffer = m_streamBuffer.GetBuffer ();
		Array.Clear (buffer, 0, buffer.Length);
		m_streamBuffer.Position = 0;
		m_streamBuffer.SetLength(0);
	}
}
