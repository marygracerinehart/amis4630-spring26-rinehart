import React from 'react';
import { useNotification } from '../../context/NotificationContext';
import '../../styles/Toast.css';

function Toast() {
  const { notifications } = useNotification();

  return (
    <div className="toast-container">
      {notifications.map((notification) => (
        <div key={notification.id} className={`toast toast-${notification.type}`}>
          <span className="toast-message">{notification.message}</span>
        </div>
      ))}
    </div>
  );
}

export default Toast;
