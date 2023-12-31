﻿using PentaGE.Core.Components;

namespace PentaGE.Core.Entities
{
    /// <summary>
    /// Provides extension methods for querying entities.
    /// </summary>
    public static class EntityCollectionExtensions
    {
        /// <summary>
        /// Filters the source collection to return entities of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of entities to consider for filtering.</typeparam>
        /// <param name="source">The collection of entities to filter.</param>
        /// <returns>An enumerable collection of filtered entities of the specified type.</returns>
        public static IEnumerable<T> Of<T>(this IEnumerable<Entity> source) where T : Entity =>
            source.OfType<T>();

        /// <summary>
        /// Filters the source collection to return entities of the same type as the specified entity.
        /// </summary>
        /// <param name="source">The collection of entities to filter.</param>
        /// <param name="entity">The entity whose type to match for filtering.</param>
        /// <returns>An enumerable collection of filtered entities of the same type as the specified entity.</returns>
        public static IEnumerable<Entity> Of(this IEnumerable<Entity> source, Entity entity) =>
            source.Where(e => e.GetType() == entity.GetType()).Cast<Entity>();

        /// <summary>
        /// Filters the source collection to return entities that have components of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of component to consider for entity filtering.</typeparam>
        /// <param name="source">The collection of entities to filter.</param>
        /// <returns>An enumerable collection of entities that have components of the specified type.</returns>
        public static IEnumerable<Entity> With<T>(this IEnumerable<Entity> source) where T : Component =>
            source.Where(e => e.Components.Has<T>());

        /// <summary>
        /// Gets an entity of a specific type with the specified unique identifier from the collection.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="source">The collection of entities to search.</param>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>The entity of the specified type with the given ID, if found; otherwise, <see langword="null"/>.</returns>
        public static T? Get<T>(this IEnumerable<Entity> source, Guid id) where T : Entity =>
            source.FirstOrDefault(e => e is T && e.ID == id) as T;

        /// <summary>
        /// Gets an entity with the specified unique identifier from the collection.
        /// </summary>
        /// <param name="source">The collection of entities to search.</param>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>The entity with the given ID, if found; otherwise, <see langword="null"/>.</returns>
        public static Entity? Get(this IEnumerable<Entity> source, Guid id) =>
            source.FirstOrDefault(e => e.ID == id);

        /// <summary>
        /// Gets all components associated with the entities in the source collection.
        /// </summary>
        /// <param name="source">The collection of entities to retrieve components from.</param>
        /// <returns>An enumerable collection of components associated with the entities.</returns>
        public static IEnumerable<Component> GetComponents(this IEnumerable<Entity> source) =>
            source.SelectMany(e => e.Components);

        /// <summary>
        /// Gets components of the specified type associated with the entities in the source collection.
        /// </summary>
        /// <typeparam name="T">The type of component to consider for filtering.</typeparam>
        /// <param name="source">The collection of entities to retrieve components from.</param>
        /// <returns>An enumerable collection of components of the specified type associated with the entities.</returns>
        public static IEnumerable<T> GetComponentsOf<T>(this IEnumerable<Entity> source) where T : Component =>
            source.SelectMany(e => e.Components.OfType<T>());
    }
}