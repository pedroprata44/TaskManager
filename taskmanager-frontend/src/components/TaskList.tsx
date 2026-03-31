import { FC, useState } from 'react';
import { TaskItem, TaskStatus } from '../types';
import { useTaskStore } from '../store/taskStore';
import { Edit2, Trash2, CheckCircle2, Circle, Clock } from 'lucide-react';
import TaskEditModal from './TaskEditModal';

interface TaskListProps {
  tasks: TaskItem[];
}

const TaskList: FC<TaskListProps> = ({ tasks }) => {
  const updateTaskStatus = useTaskStore((state) => state.updateTaskStatus);
  const deleteTask = useTaskStore((state) => state.deleteTask);
  const [editingId, setEditingId] = useState<string | null>(null);

  const handleStatusChange = (id: string, currentStatus: TaskStatus) => {
    const nextStatus = 
      currentStatus === TaskStatus.Pending 
        ? TaskStatus.InProgress 
        : currentStatus === TaskStatus.InProgress
        ? TaskStatus.Completed
        : TaskStatus.Pending;
    
    updateTaskStatus(id, nextStatus);
  };

  const getStatusIcon = (status: TaskStatus) => {
    switch (status) {
      case TaskStatus.Pending:
        return <Circle size={20} className="text-blue-500" />;
      case TaskStatus.InProgress:
        return <Clock size={20} className="text-yellow-500" />;
      case TaskStatus.Completed:
        return <CheckCircle2 size={20} className="text-green-500" />;
    }
  };

  const getStatusBadge = (status: TaskStatus) => {
    const baseClasses = 'inline-block px-3 py-1 rounded-full text-sm font-medium';
    switch (status) {
      case TaskStatus.Pending:
        return `${baseClasses} bg-blue-100 text-blue-800`;
      case TaskStatus.InProgress:
        return `${baseClasses} bg-yellow-100 text-yellow-800`;
      case TaskStatus.Completed:
        return `${baseClasses} bg-green-100 text-green-800`;
    }
  };

  return (
    <>
      <div className="divide-y">
        {tasks.map((task) => (
          <div key={task.id} className="p-6 hover:bg-gray-50 transition">
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <div className="flex items-center gap-3 mb-2">
                  <button
                    onClick={() => handleStatusChange(task.id, task.status)}
                    className="hover:opacity-70 transition"
                    title="Mudar status"
                  >
                    {getStatusIcon(task.status)}
                  </button>
                  <h3 className="text-lg font-semibold text-gray-900">{task.title}</h3>
                  <span className={getStatusBadge(task.status)}>
                    {task.status}
                  </span>
                </div>
                {task.description && (
                  <p className="text-gray-600 ml-8">{task.description}</p>
                )}
                <p className="text-sm text-gray-500 ml-8 mt-2">
                  Criado em: {new Date(task.createdAt).toLocaleDateString('pt-BR')}
                </p>
              </div>

              <div className="flex gap-2 ml-4">
                <button
                  onClick={() => setEditingId(task.id)}
                  className="p-2 hover:bg-blue-50 text-blue-600 rounded-md transition"
                  title="Editar"
                >
                  <Edit2 size={18} />
                </button>
                <button
                  onClick={() => deleteTask(task.id)}
                  className="p-2 hover:bg-red-50 text-red-600 rounded-md transition"
                  title="Deletar"
                >
                  <Trash2 size={18} />
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>

      {editingId && (
        <TaskEditModal
          task={tasks.find((t) => t.id === editingId)!}
          onClose={() => setEditingId(null)}
        />
      )}
    </>
  );
};

export default TaskList;
