import { FC, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import { useTaskStore } from '../store/taskStore';
import { TaskStatus } from '../types';
import TaskList from '../components/TaskList';
import TaskForm from '../components/TaskForm';
import { LogOut } from 'lucide-react';

const Dashboard: FC = () => {
  const navigate = useNavigate();
  const clearToken = useAuthStore((state) => state.clearToken);
  const { tasks, loading, error, fetchTasks } = useTaskStore();
  const [showForm, setShowForm] = useState(false);

  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  const handleLogout = () => {
    clearToken();
    navigate('/login');
  };

  const getTaskStats = () => {
    const pending = tasks.filter((t) => t.status === TaskStatus.Pending).length;
    const inProgress = tasks.filter((t) => t.status === TaskStatus.InProgress).length;
    const completed = tasks.filter((t) => t.status === TaskStatus.Completed).length;
    return { pending, inProgress, completed };
  };

  const stats = getTaskStats();

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow">
        <div className="max-w-6xl mx-auto px-4 py-6 flex justify-between items-center">
          <h1 className="text-3xl font-bold text-gray-900">TaskManager</h1>
          <button
            onClick={handleLogout}
            className="flex items-center gap-2 bg-red-600 text-white px-4 py-2 rounded-md hover:bg-red-700"
          >
            <LogOut size={18} />
            Sair
          </button>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-6xl mx-auto px-4 py-8">
        {/* Stats */}
        <div className="grid grid-cols-3 gap-4 mb-8">
          <div className="bg-white rounded-lg shadow p-6">
            <div className="text-3xl font-bold text-blue-600">{stats.pending}</div>
            <div className="text-gray-600">Pendentes</div>
          </div>
          <div className="bg-white rounded-lg shadow p-6">
            <div className="text-3xl font-bold text-yellow-600">{stats.inProgress}</div>
            <div className="text-gray-600">Em Progresso</div>
          </div>
          <div className="bg-white rounded-lg shadow p-6">
            <div className="text-3xl font-bold text-green-600">{stats.completed}</div>
            <div className="text-gray-600">Concluídas</div>
          </div>
        </div>

        {/* Error Message */}
        {error && (
          <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded text-red-700">
            {error}
          </div>
        )}

        {/* Add Task Button */}
        <div className="mb-8">
          <button
            onClick={() => setShowForm(!showForm)}
            className="bg-blue-600 text-white px-6 py-2 rounded-md hover:bg-blue-700 font-medium"
          >
            {showForm ? 'Cancelar' : '+ Nova Tarefa'}
          </button>
        </div>

        {/* Task Form */}
        {showForm && (
          <div className="mb-8">
            <TaskForm onClose={() => setShowForm(false)} />
          </div>
        )}

        {/* Tasks List */}
        <div className="bg-white rounded-lg shadow">
          {loading ? (
            <div className="p-8 text-center text-gray-500">Carregando tarefas...</div>
          ) : tasks.length === 0 ? (
            <div className="p-8 text-center text-gray-500">
              Nenhuma tarefa por enquanto. Crie uma nova!
            </div>
          ) : (
            <TaskList tasks={tasks} />
          )}
        </div>
      </main>
    </div>
  );
};

export default Dashboard;
