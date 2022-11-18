// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;

public class GameEvent
{

}

public class Events
{
    /// 
    /// @Descr: Singleton instance holding only object of event system
    /// 
    static Events m_instance;
    public static Events Instance
    {
        get
        {
            if ( m_instance == null )
            {
                m_instance = new Events();
            }

            return m_instance;
        }
    }

    /// 
    /// @Descr: delegate to event handler template
    /// 
    public delegate void EventDelegate<T>( T e ) where T : GameEvent;

    /// 
    /// @Descr: mapping of event class to handler function
    /// 
    readonly Dictionary<Type, Delegate> m_delegates = new Dictionary<Type, Delegate>();

    /// 
    /// @Descr: function to add new handler
    /// 
    public void AddListener<T>( EventDelegate<T> listener ) where T : GameEvent
    {
        Delegate d;
        if ( m_delegates.TryGetValue( typeof( T ), out d ) )
        {
            m_delegates[typeof( T )] = Delegate.Combine( d, listener );
        }
        else
        {
            m_delegates[typeof( T )] = listener;
        }
    }

    /// 
    /// @Descr: remove handler from map
    /// 
    public void RemoveListener<T>( EventDelegate<T> listener ) where T : GameEvent
    {
        Delegate d;
        if ( m_delegates.TryGetValue( typeof( T ), out d ) )
        {
            Delegate currentDel = Delegate.Remove(d, listener);

            if ( currentDel == null )
            {
                m_delegates.Remove( typeof( T ) );
            }
            else
            {
                m_delegates[typeof( T )] = currentDel;
            }
        }
    }

    /// 
    /// @Descr: function to raise an event of type T
    /// 
    public void Raise<T>( T e ) where T : GameEvent
    {
        if ( e == null )
        {
            throw new ArgumentNullException( "e" );
        }

        Delegate d;
        if ( m_delegates.TryGetValue( typeof( T ), out d ) )
        {
            EventDelegate<T> callback = d as EventDelegate<T>;
            if ( callback != null )
            {
                callback( e );
            }
        }
    }
}