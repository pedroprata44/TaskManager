import { FC, FormEvent, useState } from 'react';
import { useTaskStore } from '../store/taskStore';

interface TaskFormProps {
  onClose: () => void;
}

const TaskForm: FC<TaskFormProps> = ({ onClose }) => {
  const addTask = useTaskStore((state) => state.addTask);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setLoading(true);

    try {
      await addTask(title, description);
      setTitle('');
      setDescription('');
      onClose();
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow p-6 space-y-4">
      <div>
        <label htmlFor="task-title" className="block text-sm font-medium text-gray-700 mb-1">
          Título
        </label>
        <input
          id="task-title"
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          placeholder="Digite o título da tarefa"
          required
        />
      </div>

      <div>
        <label htmlFor="task-description" className="block text-sm font-medium text-gray-700 mb-1">
          Descrição
        </label>
        <textarea
          id="task-description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          placeholder="Digite a descrição (opcional)"
          rows={3}
        />
      </div>

      <div className="flex gap-2">
        <button
          type="submit"
          disabled={loading}
          className="flex-1 bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 disabled:bg-blue-400 font-medium"
        >
          {loading ? 'Criando...' : 'Criar Tarefa'}
        </button>
        <button
          type="button"
          onClick={onClose}
          className="px-6 py-2 border border-gray-300 rounded-md hover:bg-gray-50 font-medium"
        >
          Cancelar
        </button>
      </div>
    </form>
  );
};

export default TaskForm;
